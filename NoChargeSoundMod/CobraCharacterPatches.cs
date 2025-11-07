using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoChargeSound;

[HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.UpdateChargedShotHoldTimer))]
public static class CobraCharacterPatches
{
    private const audioSelectionData.eCLIP ChargingSound = audioSelectionData.eCLIP.PLY_SHOOT_PG_CHARGING;
    private const audioSelectionData.eCLIP ChargedSound = audioSelectionData.eCLIP.PLY_SHOOT_PG_CHARGED;
    private const audioSelectionData.eCLIP JustChargedSound = audioSelectionData.eCLIP.PLY_SHOOT_PG_JUSTCHARGED;
    
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.Start()
            .MatchForward(true, new CodeMatch(OpCodes.Ldc_I4, (int)JustChargedSound))
            .ThrowIfInvalid("Failed to found the just charged sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraCharacterPatches), nameof(MultiplyVolumeForJustCharged)))
            .MatchForward(true, new CodeMatch(OpCodes.Ldc_I4, (int)ChargedSound))
            .ThrowIfInvalid("Failed to found the charged sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraCharacterPatches), nameof(MultiplyVolumeForCharged)))
            .MatchForward(true, new CodeMatch(OpCodes.Ldc_I4, (int)ChargingSound))
            .ThrowIfInvalid("Failed to found the charging sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraCharacterPatches), nameof(MultiplyVolumeForCharging)));
        
        return codeMatcher.Instructions();
    }

    private static float MultiplyVolumeForCharging(float originalVolume) => originalVolume * Plugin.ChargingSoundVolumeMultiplier.Value;
    private static float MultiplyVolumeForCharged(float originalVolume) => originalVolume * Plugin.ChargedSoundVolumeMultiplier.Value;
    private static float MultiplyVolumeForJustCharged(float originalVolume) => originalVolume * Plugin.JustChargedSoundVolumeMultiplier.Value;
}