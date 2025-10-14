using HarmonyLib;
using UnityEngine;

namespace RevolverFix;

[HarmonyPatch]
public static class Patches
{
    private static readonly int FloatMetalness = Shader.PropertyToID("_Float_metalness");
    private static readonly int AlbedoTextureProperty = Shader.PropertyToID("_Texture2D_alb");
    private const string CobraMaterialName = "mat_chr_Cobra_00";
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Start))]
    public static void FixRevolverPatch(CobraCharacter __instance)
    {
        foreach (var material in __instance.defaultMats)
        {
            if (material != null && material.name == CobraMaterialName)
            {
                material.SetFloat(FloatMetalness, 0);
                if (Plugin.NewTexture != null)
                {
                    material.SetTexture(AlbedoTextureProperty, Plugin.NewTexture);
                }
                else
                {
                    Plugin.Logger.LogError("Failed to find new cobra texture!");
                }
                return;
            }
        }
        Plugin.Logger.LogWarning("Material not found!");
    }
}