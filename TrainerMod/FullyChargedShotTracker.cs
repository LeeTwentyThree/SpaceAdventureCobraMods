namespace TrainerMod;

public static class FullyChargedShotTracker
{
    private static bool _fullyCharged;
    
    public static bool GetDidPlayerShootFullyChargedShot()
    {
        return _fullyCharged;
    }
    
    public static void OnShoot(bool fullyCharged)
    {
        _fullyCharged = fullyCharged;
    }
}