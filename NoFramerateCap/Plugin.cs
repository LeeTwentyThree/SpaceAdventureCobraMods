using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace NoFramerateCap;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    public static ConfigEntry<int> FrameRate;
    public static ConfigEntry<bool> DisableVsync;

    private void Awake()
    {
        FrameRate = Config.Bind(
            "General",
            "NewFrameRate",
            -1,
            "The new target framerate for the game. A value of -1 means there is no limit. Restart required."
        );
        
        DisableVsync = Config.Bind(
            "General",
            "DisableVsync",
            true,
            "If true, vsync will be disabled."
        );
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
