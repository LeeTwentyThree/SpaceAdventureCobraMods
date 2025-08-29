using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace NoChargeSound;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static ConfigEntry<float> SoundVolumeMultiplier;
    
    private void Awake()
    {
        Logger = base.Logger;

        SoundVolumeMultiplier = Config.Bind("General",
            "SoundVolumeMultiplier",
            0f,
            "The volume multiplier for the charge sound."
        );

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
