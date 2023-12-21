using System.Reflection;
using BepInEx.Configuration;
using BrutalCompanyPlus.Utils;

namespace BrutalCompanyPlus.Config;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ConfigCategory : System.Attribute {
    public readonly string Name;
    public ConfigCategory(string Name) => this.Name = Name;
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class SharedDescription : System.Attribute {
    public readonly string Template;
    public SharedDescription(string Template) => this.Template = Template;
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class Configuration<T> : System.Attribute where T : notnull {
    public string Name { get; internal set; }
    public ConfigDescription Description { get; internal set; }
    public readonly T DefaultValue;
    internal readonly bool ExcludeFromShared;

    public Configuration(string Name, ConfigDescription Description, T DefaultValue, bool ExcludeFromShared = false) {
        this.Name = Name;
        this.Description = Description;
        this.DefaultValue = DefaultValue;
    }

    public Configuration(string Description, T DefaultValue, bool ExcludeFromShared = false) :
        this(null, new ConfigDescription(Description), DefaultValue, ExcludeFromShared) { }

    public override string ToString() =>
        $"(name: {Name}, description: {Description}, default: {DefaultValue}, type: {typeof(T).Name})";
}

public static class ConfigLoader {
    public static void Bind(Plugin Plugin) {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            if (!type.TryGetAttribute(out ConfigCategory category)) continue;
            Plugin.Logger.LogWarning($"Binding config for {type.Name}...");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            Plugin.Logger.LogWarning($"Found {properties.Length} properties...");
            foreach (var prop in properties) {
                if (!type.TryGetAttribute(out Configuration<object> config)) continue;
                Plugin.Logger.LogWarning($"Binding config for {type.Name}...");

                if (string.IsNullOrEmpty(config.Name)) config.Name = prop.Name;
                if (!config.ExcludeFromShared && type.TryGetAttribute(out SharedDescription description)) {
                    config.Description = new ConfigDescription(
                        description.Template.Replace("{}", config.Description.Description),
                        config.Description.AcceptableValues,
                        config.Description.Tags
                    );
                }

                Plugin.Logger.LogWarning($"Binding config for {type.Name}/{prop.Name}: {config}");
                var entry = Plugin.Config.Bind(category.Name, config.Name, config.DefaultValue, config.Description);
                prop.SetValue(null, entry);
            }
        }
    }

    private static bool TryGetAttribute<T>(this MemberInfo Member, out T Attribute) where T : System.Attribute {
        var attrs = Member.GetCustomAttributes(typeof(T), false);
        if (attrs.IsEmpty()) {
            Attribute = default;
            return false;
        }

        Attribute = (T)attrs[0];
        return true;
    }
}