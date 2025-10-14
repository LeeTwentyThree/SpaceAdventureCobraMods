using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RevolverFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static Texture2D NewTexture { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;

        var assembly = Assembly.GetExecutingAssembly();
        Harmony.CreateAndPatchAll(assembly);

        // LOAD TEXTURE
        
        var modFolder = Path.GetDirectoryName(assembly.Location);
        var planetFilePath = Path.Combine(modFolder, "NewCobraTexture.png");

        byte[] fileData = File.ReadAllBytes(planetFilePath);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(fileData))
        {
            Logger.LogWarning("Failed to decode texture!");
            return;
        }

        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;
        NewTexture = texture;

        // LOG COMPLETION!
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}