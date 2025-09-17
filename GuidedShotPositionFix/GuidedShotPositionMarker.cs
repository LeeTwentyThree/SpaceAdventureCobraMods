using System.Collections.Generic;
using UnityEngine;

namespace GuidedShotPositionFix;

public class GuidedShotPositionMarker : MonoBehaviour
{
    private static readonly HashSet<GuidedShotPositionMarker> Markers = [];

    private Transform _shootTransform;
    private CobraCharacter _cobra;

    private static readonly FloatGradient Angles = new([
        new(0f / 360f, 0.03f), // 0° → 0.03
        new(45f / 360f, 0.05f), // 45° → 0.05
        new(90f / 360f, 0.00f), // 90° → 0
        new(135f / 360f, 0.03f), // 135° → 0.03
        new(180f / 360f, 0.03f), // 180° → 0.03
        new(315f / 360f, -0.02f), // 315° → -0.02
        new(360f / 360f, 0.03f) // 360° → 0.03
    ]);

    private void OnEnable()
    {
        if (!_cobra)
            _cobra = GetComponent<CobraCharacter>();
        Markers.Add(this);
    }

    private void OnDisable()
    {
        Markers.Remove(this);
    }

    private Transform GetShootTransform()
    {
        if (_shootTransform != null)
        {
            return _shootTransform;
        }

        var parent = transform.FindRecursive("forearm_L");
        if (parent == null)
        {
            Plugin.Logger.LogWarning("Failed to find forearm position!");
            return null;
        }

        _shootTransform = new GameObject("FixedGuidedShotShootTransform").transform;
        _shootTransform.SetParent(parent);
        _shootTransform.localPosition = new Vector3(-0.4f, 0.05f, 0);
        return _shootTransform;
    }

    public static bool TryGetProperShootPosition(string cobraName, out Vector3 position)
    {
        if (Markers.Count == 0)
        {
            Plugin.Logger.LogWarning("No Cobras in scene. Cannot get proper shoot position.");
            position = default;
            return false;
        }

        foreach (var marker in Markers)
        {
            if (marker == null) continue;
            if (marker.gameObject.name != cobraName) continue;
            var shootTransform = marker.GetShootTransform();
            if (shootTransform == null)
            {
                Plugin.Logger.LogWarning("Shoot transform could not be found!");
                continue;
            }

            var shootTransformLocalPosition = shootTransform.localPosition;
            shootTransformLocalPosition.y = AngleToYOffset(marker._cobra.armAngle);
            shootTransform.localPosition = shootTransformLocalPosition;
            
            position = shootTransform.position;
            return true;
        }

        position = default;
        return false;
    }

    private static float AngleToYOffset(float angle)
    {
        float wrappedAngle = ((angle % 360f) + 360f) % 360f;
        float normalizedAngle = wrappedAngle / 360f;
        return Angles.Evaluate(normalizedAngle);
    }
}