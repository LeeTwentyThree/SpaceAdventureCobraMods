using CobraSoundReplacer.Core;

namespace CobraSoundReplacer.API;

public static class CustomSoundUtils
{
    public static bool TryGetIdForCustomSound(string clipName, out ushort id)
    {
        return SoundPackRegistry.NewSoundsIDs.TryGetValue(clipName, out id);
    }
}