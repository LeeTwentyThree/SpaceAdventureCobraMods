using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace Cobra1982AnimeSoundPatches;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    internal static ConfigEntry<float> PyschogunShotDecibelGain { get; private set; }
    internal static ConfigEntry<float> SuperShotDecibelGain { get; private set; }
    internal static ConfigEntry<bool> DisableChargeSoundLoop { get; private set; }
    internal static AudioMixerGroup PsychogunMixerGroup { get; private set; }
    internal static AudioMixerGroup SuperShotMixerGroup { get; private set; }
    
    internal static AssetBundle Bundle { get; private set; }


    private void Awake()
    {
        Logger = base.Logger;

        var assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(assembly);
        
        PyschogunShotDecibelGain = Config.Bind("General", "Psychogun shot decibel gain", 1.5f);
        SuperShotDecibelGain = Config.Bind("General", "Super shot decibel gain", 0f);
        DisableChargeSoundLoop = Config.Bind("General", "Disable charge sound loop", true);
        
        try
        {
            var bundleName = "1982animesoundassets";
            var psychogunMixerName = "NormalPsychogunShotMixer";
            var superShotMixerName = "SuperShotMixer";
            var bundlePath = Path.Combine(Path.GetDirectoryName(assembly.Location), bundleName); 

            if (!File.Exists(bundlePath))
            {
                Logger.LogError($"AssetBundle not found at: {bundlePath}");
                return;
            }

            Bundle = AssetBundle.LoadFromFile(bundlePath);
            if (Bundle == null)
            {
                Logger.LogError("Failed to load AssetBundle!");
                return;
            }

            var psychogunMixer = Bundle.LoadAsset<AudioMixer>(psychogunMixerName);
            if (psychogunMixer == null)
            {
                Logger.LogError($"AudioMixer '{psychogunMixerName}' not found in bundle!");
                return;
            }

            var groups = psychogunMixer.FindMatchingGroups("Master");
            if (groups.Length > 0)
            {
                PsychogunMixerGroup = groups[0];
                Logger.LogInfo($"Loaded mixer group: {PsychogunMixerGroup.name}");
            }
            else
            {
                Logger.LogWarning("No 'Master' group found in the mixer!");
            }
            
            var superShotMixer = Bundle.LoadAsset<AudioMixer>(superShotMixerName);
            if (superShotMixer == null)
            {
                Logger.LogError($"AudioMixer '{superShotMixerName}' not found in bundle!");
                return;
            }

            var superShotGroups = superShotMixer.FindMatchingGroups("Master");
            if (superShotGroups.Length > 0)
            {
                SuperShotMixerGroup = superShotGroups[0];
                Logger.LogInfo($"Loaded mixer group: {SuperShotMixerGroup.name}");
            }
            else
            {
                Logger.LogWarning("No 'Master' group found in the mixer!");
            }
        }
        catch (System.Exception e)
        {
            Logger.LogError("Error loading AudioMixer: " + e);
        }

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}