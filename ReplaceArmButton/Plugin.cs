using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ReplaceArmButton;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static ConfigEntry<KeyCode> KeyboardBinding;
    public static ConfigEntry<DPadDirection> ControllerDPadOption;
    
    private void Awake()
    {
        Logger = base.Logger;

        KeyboardBinding = Config.Bind("Controls",
            "KeyboardBinding",
            KeyCode.T,
            "The button that replaces the arm on PC."
        );
        
        ControllerDPadOption = Config.Bind("Controls",
            "DPadButton",
            DPadDirection.Left,
            "The D-pad button that replaces the arm when playing on controllers."
        );


        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
