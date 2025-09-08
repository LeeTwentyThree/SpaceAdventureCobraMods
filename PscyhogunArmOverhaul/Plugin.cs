using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PscyhogunArmOverhaul;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static ConfigEntry<KeyCode> KeyboardBinding;
    public static ConfigEntry<DPadDirection> ControllerDPadOption;
    public static ConfigEntry<float> ArmOffSound;
    
    internal static AssetBundle Bundle { get; private set; }
    internal static Assembly ModAssembly { get; } = Assembly.GetExecutingAssembly();
    
    private void Awake()
    {
        Logger = base.Logger;
        
        Bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(ModAssembly.Location), "000arm_mod"));

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

        ArmOffSound = Config.Bind("Audio",
            "ArmOffSoundVolume",
            0.5f,
            "The volume of the sound when taking off the prosthetic arm.");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
