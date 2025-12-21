using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace TrainerMod.Patches;

[HarmonyPatch]
public static class SuperShotDamagePatches
{
    private static readonly List<IDamageable> StoredDamageables = [];
    private static SuperGuidedProjectileV2 _storedSuperShot;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SuperGuidedProjectileV2), nameof(SuperGuidedProjectileV2.EndOfSuperGuided))]
    private static void ClearTarget(SuperGuidedProjectileV2 __instance)
    {
        StoredDamageables.Clear();
        _storedSuperShot = __instance;
        if (__instance.targetsAsDmgAble != null)
        {
            StoredDamageables.AddRange(__instance.targetsAsDmgAble);
            __instance.targetsAsDmgAble.Clear();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.GuidedShotOver))]
    private static void DamageStoredTargets(bool wasSuper)
    {
        if (!wasSuper) return;

        try
        {
            int num = 0;
            foreach (IDamageable damageable in StoredDamageables)
            {
                bool survivesSuperShot = false;
                var boss = damageable as INmiBoss;
                var enemy = damageable as NmiBasic;
                if (boss != null || (enemy != null &&
                                     (enemy.common.flags &
                                      (NmiBasic.Common.Flag.MiniBoss
                                       | NmiBasic.Common.Flag.Elite)) != 0))
                {
                    survivesSuperShot = true;
                }

                if (enemy != null || damageable is INmi)
                {
                    num++;
                }

                if (_storedSuperShot.targetFX.vfx != null)
                {
                    _storedSuperShot.targetFX.PlayWorld(damageable.GetCenter(), Quaternion.identity, doStop: false);
                }

                var eliteDamage = Plugin.SuperShotDamageElites.Value >= 0
                    ? Plugin.SuperShotDamageElites.Value
                    : Plugin.DefaultSuperShotDamageElites;
                var normalDamage = Plugin.SuperShotDamageNormal.Value >= 0
                    ? Plugin.SuperShotDamageNormal.Value
                    : Plugin.DefaultSuperShotDamageNormal;
                damageable.TakeDamage(_storedSuperShot.gameObject, survivesSuperShot ? eliteDamage : normalDamage, 0,
                    _storedSuperShot.damageType, Vector3.right, damageable.GetCenter(),
                    null);
            }
            
            if (num >= 6)
            {
                AchievementsController.RegisterEvent(AchievementsController.EAchievementType.SUPERGUIDEDSHOT_SKEWER);
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
        finally
        {
            _storedSuperShot = null;
            StoredDamageables.Clear();
        }
    }
}