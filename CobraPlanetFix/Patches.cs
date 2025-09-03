using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CobraPlanetFix;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NUIMainMenu), nameof(NUIMainMenu.Start))]
    private static void MainMenuStartPostfix(NUIMainMenu __instance)
    {
        var planetSprite = GetNewPlanetSprite();
        var planet = __instance.transform.Find("planet");
        var image = planet.GetComponent<Image>();
        
        image.sprite = planetSprite;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UITitleController), nameof(UITitleController.Start))]
    private static void UITitleControllerStartPostfix(UITitleController __instance)
    {
        var planetSprite = GetNewPlanetSprite();
        var planet = __instance.transform.Find("planet");
        var image = planet.GetComponent<Image>();
        
        image.sprite = planetSprite;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NUILuminositySetupPanel), nameof(NUILuminositySetupPanel.Start))]
    private static void NuiLuminositySetupPanelStartPostfix(NUILuminositySetupPanel __instance)
    {
        var planetSprite = GetNewPlanetSprite();
        var planet = __instance.transform.Find("planet");
        var image = planet.GetComponent<Image>();
        
        image.sprite = planetSprite;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadScene))]
    private static void SceneLoaderStartPostfix(SceneLoader __instance)
    {
        Sprite planetSprite = null;
        var images = Object.FindObjectsOfType<Image>();
        foreach (var image in images)
        {
            if (!image.enabled)
                continue;
            if (!image.name.Equals("planet"))
                continue;
            if (planetSprite == null)
                planetSprite = GetNewPlanetSprite();
            image.sprite = planetSprite;
        }
    }

    private static Sprite GetNewPlanetSprite()
    {
        try
        {
            var modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var planetFilePath = Path.Combine(modFolder, "ui_planet.png");

            byte[] fileData = File.ReadAllBytes(planetFilePath);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(fileData))
            {
                Plugin.Logger.LogWarning("Failed to decode planet!");
                return null;
            }

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );

            return sprite;
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Exception while loading planet texture: " + e);
            return null;
        }
    }
}