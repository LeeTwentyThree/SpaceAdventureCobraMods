using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DodgingTweaks;

[HarmonyPatch(typeof(CobraCharacter))]
public static class CobraPatches
{
    private const audioSelectionData.eCLIP StandingStillDodgeSound = audioSelectionData.eCLIP.PLY_DODGESTATIC;
    private const audioSelectionData.eCLIP InAirDodgeSound = audioSelectionData.eCLIP.PLY_DODGEINAIR;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CobraCharacter.Start))]
    private static void StartPostfix(CobraCharacter __instance)
    {
        if (Plugin.DisableDodgeVfx.Value)
        {
            var dependencies = __instance.dependencies;
            dependencies.successfulDodgeMat = __instance.defaultMats[0];
        }

        if (Plugin.DisableDodgeTrailVfx.Value)
        {
            var particles = __instance.particles;
            var afterGlow = particles.FirstOrDefault(p => p.name == "afterglow");
            if (afterGlow != null)
            {
                foreach (var renderer in afterGlow.particle.GetComponentsInChildren<Renderer>())
                    renderer.enabled = false;
            }
            else
            {
                Plugin.Logger.LogWarning("Failed to find afterglow particle effect!");
            }   
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(CobraCharacter.UpdateStandingState))]
    private static IEnumerable<CodeInstruction> UpdateStandingStateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.Start()
            .MatchForward(true, new CodeMatch(i => IsSoundInstruction(i, StandingStillDodgeSound)))
            .ThrowIfInvalid("Failed to find the standing still dodge sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraPatches), nameof(MultiplyVolumeForStandingStillDodge)));

        return codeMatcher.Instructions();
    }
    
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(CobraCharacter.UpdateCrouchedState))]
    private static IEnumerable<CodeInstruction> UpdateCrouchedStateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.Start()
            .MatchForward(true, new CodeMatch(i => IsSoundInstruction(i, StandingStillDodgeSound)))
            .ThrowIfInvalid("Failed to find the standing still dodge sound!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraPatches), nameof(MultiplyVolumeForStandingStillDodge)));

        return codeMatcher.Instructions();
    }
    
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(CobraCharacter.UpdateInAirState))]
    private static IEnumerable<CodeInstruction> UpdateInAirStateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.Start()
            .MatchForward(true, new CodeMatch(i => IsSoundInstruction(i, InAirDodgeSound)))
            .ThrowIfInvalid("Failed to find the in air dodge dodge sound #1!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraPatches), nameof(MultiplyVolumeForInAirDodge)))
            .MatchForward(true, new CodeMatch(i => IsSoundInstruction(i, InAirDodgeSound)))
            .ThrowIfInvalid("Failed to find the in air dodge dodge sound #2!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraPatches), nameof(MultiplyVolumeForInAirDodge)))
            .MatchForward(true, new CodeMatch(i => IsSoundInstruction(i, InAirDodgeSound)))
            .ThrowIfInvalid("Failed to find the in air dodge dodge sound #3!")
            .Advance(2)
            .Insert(CodeInstruction.Call(typeof(CobraPatches), nameof(MultiplyVolumeForInAirDodge)));

        return codeMatcher.Instructions();
    }
    
    private static bool IsSoundInstruction(CodeInstruction instruction, audioSelectionData.eCLIP sound)
    {
        if (instruction.opcode != OpCodes.Ldc_I4 && instruction.opcode != OpCodes.Ldc_I4_S)
            return false;

        int val;
        if (instruction.operand is int intVal) val = intVal;
        else if (instruction.operand is ushort shortVal) val = shortVal;
        else if (instruction.operand is sbyte sbyteVal) val = sbyteVal;
        else return false;

        return val == (ushort)sound;
    }

    private static float MultiplyVolumeForStandingStillDodge(float originalVolume) =>
        Plugin.StandingStillDodgeSoundVolumeMultiplier.Value * originalVolume;

    private static float MultiplyVolumeForInAirDodge(float originalVolume) =>
        Plugin.InAirDodgeSoundVolumeMultiplier.Value * originalVolume;
}