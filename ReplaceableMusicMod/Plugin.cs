using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MusicReplacer.ReplacementSystem;

namespace MusicReplacer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.lee23.cobra1982animesoundpatches")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static Assembly Assembly { get; private set; }
    
    private void Awake()
    {
        Assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(Assembly);

        // Plugin startup logic
        Logger = base.Logger;
        
        // Register main logic
        MusicReplacementManager.Register();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
