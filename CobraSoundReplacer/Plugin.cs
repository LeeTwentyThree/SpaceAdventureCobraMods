using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using CobraSoundReplacer.Core;
using CobraSoundReplacer.Patches;
using HarmonyLib;

namespace CobraSoundReplacer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private static Plugin _instance;
        
    private void Awake()
    {
        _instance = this;
        
        Logger = base.Logger;
        
        var harmony = Harmony.CreateAndPatchAll(Assembly, $"{MyPluginInfo.PLUGIN_GUID}");
        RegisterManualPatches(harmony);
        
        StartCoroutine(SoundPackRegistry.LoadPacksAutomatically());
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public static void StartCoroutineOnPlugin(IEnumerator coroutine)
    {
        _instance.StartCoroutine(coroutine);
    }

    private static void RegisterManualPatches(Harmony harmony)
    {
        
    }
}
