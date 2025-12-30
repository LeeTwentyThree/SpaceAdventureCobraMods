using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace IntroSwapMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static VideoDatabase VideoDatabase { get; private set; }
    
    internal static ConfigEntry<string> BundleName { get; set; }
    internal static ConfigEntry<float> VideoVolume { get; set; }

    private const string VideosFolder = "Videos";
    
    private void Awake()
    {
        Logger = base.Logger;

        var assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(assembly);

        var videoFolder = Path.Combine(Path.GetDirectoryName(assembly.Location), VideosFolder);
        VideoDatabase = new VideoDatabase(videoFolder);

        var options = new List<string> {"none"};
        if (!Directory.Exists(videoFolder))
        {
            Logger.LogWarning("Video folder doesn't exist at path " + videoFolder);
        }
        else
        {
            foreach (var file in Directory.GetFiles(videoFolder))
            {
                if (!file.Contains("."))
                    options.Add(Path.GetFileName(file));
            }
        }

        var joinedOptionsString = options.Join();
        var normalOptionsString = "none, original, swedish, swedish_2";
        if (!string.IsNullOrEmpty(joinedOptionsString) && !normalOptionsString.Equals(joinedOptionsString))
        {
            normalOptionsString = joinedOptionsString;
        }
        
        BundleName = Config.Bind("General", "Video file name",
            "original",
            new ConfigDescription("The name of the file from the plugin's 'Videos' folder to load. Options: " + normalOptionsString));

        VideoVolume = Config.Bind("General", "Video volume",
            0.4f,
            new ConfigDescription("The volume multiplier for the video."));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}