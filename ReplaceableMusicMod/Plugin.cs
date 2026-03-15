using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MusicReplacer.ReplacementSystem;

namespace MusicReplacer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.lee23.cobrasoundreplacer", "1.3.1")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static Assembly Assembly { get; private set; }

    private static Plugin _main;
    
    private void Awake()
    {
        _main = this;
        
        Assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(Assembly);

        // Plugin startup logic
        Logger = base.Logger;
        
        // Register main logic
        MusicReplacementManager.Register();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public static void StartCoroutineOnPlugin(IEnumerator coroutine)
    {
        _main.StartCoroutine(coroutine);
    }
}
