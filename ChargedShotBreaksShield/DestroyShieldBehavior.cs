using UnityEngine;

namespace ChargedShotsBreakShields;

public class DestroyShieldBehavior : MonoBehaviour
{
    private const float DestroyRevolverShieldRadius = 1.5f;

    private bool _fullyCharged;

    private static readonly Collider[] SharedBuffer = new Collider[32];

    private void Start()
    {
        _fullyCharged = FullyChargedShotTracker.GetDidPlayerShootFullyChargedShot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_fullyCharged)
            return;

        var protection = other.GetComponentInParent<NmiProtection>();
        if (protection != null)
        {
            protection.ManageDestruction(true);
        }
    }

    private void FixedUpdate()
    {
        if (!_fullyCharged)
            return;

        var hits = Physics.OverlapSphereNonAlloc(transform.position, DestroyRevolverShieldRadius, SharedBuffer, -1);
        for (int i = 0; i < hits; i++)
        {
            if (SharedBuffer[i] == null)
                continue;
            var protection = SharedBuffer[i].GetComponentInParent<NmiProtection>();
            if (protection != null)
            {
                protection.ManageDestruction(true);
            }
        }
    }
}