namespace BrutalCompanyPlus.Utils;

public static class Singleton {
    public static Terminal Terminal => UnityEngine.Object.FindObjectOfType<Terminal>();
}