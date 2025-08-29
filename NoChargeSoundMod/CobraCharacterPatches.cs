using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoChargeSound;

[HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.UpdateChargedShotHoldTimer))]
public static class CobraCharacterPatches
{
    private const audioSelectionData.eCLIP SoundToReplace = audioSelectionData.eCLIP.PLY_SHOOT_PG_CHARGING;
    
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.Start()
            .MatchForward(true, new CodeMatch(OpCodes.Ldc_I4, (int)SoundToReplace))
            .ThrowIfInvalid("Failed to found the referenced sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraCharacterPatches), nameof(MultiplyVolume)));
        
        return codeMatcher.Instructions();
    }

    private static float MultiplyVolume(float originalVolume)
    {
        return originalVolume * Plugin.SoundVolumeMultiplier.Value;
    }
}