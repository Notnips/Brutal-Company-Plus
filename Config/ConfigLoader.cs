using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using BrutalCompanyPlus.Utils;

namespace BrutalCompanyPlus.Config;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigCategory : Attribute {
    public readonly string Name;
    public ConfigCategory(string Name) => this.Name = Name;
}

[AttributeUsage(AttributeTargets.Class)]
public class SharedDescription : Attribute {
    public readonly string Template;
    public SharedDescription(string Template) => this.Template = Template;
}

[AttributeUsage(AttributeTargets.Property)]
public class Configuration : Attribute {
    public string Name { get; internal set; }
    public ConfigDescription Description { get; internal set; }
    public readonly object DefaultValue;
    internal readonly bool ExcludeFromShared;

    private Configuration(string Name, ConfigDescription Description, object DefaultValue, bool ExcludeFromShared) {
        this.Name = Name;
        this.Description = Description;
        this.DefaultValue = DefaultValue;
        this.ExcludeFromShared = ExcludeFromShared;
    }

    public Configuration(string Description, object DefaultValue, bool ExcludeFromShared = false) :
        this(null, new ConfigDescription(Description), DefaultValue, ExcludeFromShared) { }

    public Configuration(string Description, int DefaultValue, int MinValue, int MaxValue,
        bool ExcludeFromShared = false) : this(null, new ConfigDescription(
        Description, new AcceptableValueRange<int>(MinValue, MaxValue)
    ), DefaultValue, ExcludeFromShared) { }

    public override string ToString() =>
        $"(name: {Name}, description: {Description.Description}, default: {DefaultValue})";
}

public static class ConfigLoader {
    public static void Bind(Plugin Plugin) {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            if (!type.TryGetAttribute(out ConfigCategory category)) continue;
            Plugin.Logger.LogDebug($"Binding config for {type.Name}...");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            Plugin.Logger.LogDebug($"Found {properties.Length} properties...");
            foreach (var prop in properties) {
                if (!prop.TryGetAttribute(out Configuration config)) continue;
                Plugin.Logger.LogDebug($"Binding config for {prop.Name}...");

                if (string.IsNullOrEmpty(config.Name)) config.Name = prop.Name;
                if (!config.ExcludeFromShared && type.TryGetAttribute(out SharedDescription description)) {
                    config.Description = new ConfigDescription(
                        description.Template.Replace("{}", config.Description.Description),
                        config.Description.AcceptableValues,
                        config.Description.Tags
                    );
                }

                if (!prop.TryGetEntryType(out var entryType)) continue;
                var defaultValue = Convert.ChangeType(config.DefaultValue, entryType);

                Plugin.Logger.LogDebug($"Binding config for {type.Name}/{prop.Name}: {config}");
                var entry = typeof(ConfigFile).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(M =>
                        M.Name == "Bind" && M.GetParameters().Length == 4 &&
                        M.GetParameters()[3].ParameterType == typeof(ConfigDescription))!.MakeGenericMethod(entryType)
                    .Invoke(Plugin.Config, new[] { category.Name, config.Name, defaultValue, config.Description });
                prop.SetValue(null, entry);
            }
        }
    }

    private static bool TryGetAttribute<T>(this MemberInfo Member, out T Attribute) where T : Attribute {
        var attrs = Member.GetCustomAttributes(typeof(T), false);
        if (attrs.IsEmpty()) {
            Attribute = default;
            return false;
        }

        Attribute = (T)attrs[0];
        return true;
    }

    private static bool TryGetEntryType(this PropertyInfo Info, out Type Type) {
        if (!Info.PropertyType.IsGenericType || Info.PropertyType.GetGenericTypeDefinition() != typeof(ConfigEntry<>)) {
            Type = default;
            return false;
        }

        var types = Info.PropertyType.GetGenericArguments();
        if (types.Length != 1) {
            Type = default;
            return false;
        }

        Type = types[0];
        return true;
    }
}