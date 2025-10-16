using CobraSoundReplacer.Core;

namespace CobraSoundReplacer.API;

public static class CustomSoundUtils
{
    public static bool TryGetIdForCustomSound(string clipName, out ushort id)
    {
        return SoundPackRegistry.NewSoundsIDs.TryGetValue(clipName, out id);
    }
    
    public static bool TryGetEClip(string eClipId, out audioSelectionData.eCLIP eClip)
    {
        return SoundPackRegistry.CustomEClips.TryGetValue(eClipId, out eClip);
    }
}