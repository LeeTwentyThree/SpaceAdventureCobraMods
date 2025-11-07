using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace Cobra1982AnimeSoundPatches;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    internal static ConfigEntry<bool> DisableChargeSoundLoop { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;

        var assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(assembly);

        DisableChargeSoundLoop = Config.Bind("General", "Disable charge sound loop", true);

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}