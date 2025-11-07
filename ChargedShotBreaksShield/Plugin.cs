using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace ChargedShotsBreakShields;

[BepInDependency("com.lee23.cobrasoundreplacer")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    internal static ConfigEntry<float> PenetratingShotDecibelGain { get; private set; }
    internal static AudioMixerGroup LoudSoundMixerGroup { get; private set; }
    
    private void Awake()
    {
        Logger = base.Logger;

        var assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(assembly);
        
        PenetratingShotDecibelGain = Config.Bind("General", "Penetrating Shot Decibel Gain", 3f);
        
        try
        {
            string bundleName = "chargedshotsbreakshield";
            string assetName = "LouderGroup";
            string bundlePath = Path.Combine(Path.GetDirectoryName(assembly.Location), bundleName); 
            // (or use Path.Combine(Paths.PluginPath, "YourPluginFolder", bundleName) if it's in a subfolder)

            if (!File.Exists(bundlePath))
            {
                Logger.LogError($"AssetBundle not found at: {bundlePath}");
                return;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Logger.LogError("Failed to load AssetBundle!");
                return;
            }

            AudioMixer mixer = bundle.LoadAsset<AudioMixer>(assetName);
            if (mixer == null)
            {
                Logger.LogError($"AudioMixer '{assetName}' not found in bundle!");
                return;
            }

            // Get the "Master" group from the mixer
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("Master");
            if (groups.Length > 0)
            {
                LoudSoundMixerGroup = groups[0];
                Logger.LogInfo($"Loaded mixer group: {LoudSoundMixerGroup.name}");
            }
            else
            {
                Logger.LogWarning("No 'Master' group found in the mixer!");
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"Error loading AudioMixer: {ex}");
        }

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}