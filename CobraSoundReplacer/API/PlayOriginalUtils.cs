namespace CobraSoundReplacer.API;

public static class PlayOriginalUtils
{
    internal static bool NextSoundPlaysOriginal { get; private set; }
    
    public static void PlayOriginalForNextSoundEvent()
    {
        NextSoundPlaysOriginal = true;
    }

    internal static void OnAnySoundPlayed()
    {
        NextSoundPlaysOriginal = false;
    }
}