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

    public static ConfigEntry<float> ChargingSoundVolumeMultiplier;
    public static ConfigEntry<float> ChargedSoundVolumeMultiplier;
    public static ConfigEntry<float> JustChargedSoundVolumeMultiplier;
    
    private void Awake()
    {
        Logger = base.Logger;

        ChargingSoundVolumeMultiplier = Config.Bind("General",
            "ChargeSoundVolumeMultiplier",
            1f,
            "The volume multiplier for the charging sound."
        );
        
        ChargedSoundVolumeMultiplier = Config.Bind("General",
            "ChargedSoundVolumeMultiplier",
            0f,
            "The volume multiplier for the charged sound."
        );

        JustChargedSoundVolumeMultiplier = Config.Bind("General",
            "JustChargedSoundVolumeMultiplier",
            0f,
            "The volume multiplier for the 'just charged' sound."
        );
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
