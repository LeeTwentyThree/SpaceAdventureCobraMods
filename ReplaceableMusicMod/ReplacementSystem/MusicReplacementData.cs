using System.Collections.Generic;
using CobraSoundReplacer.Core;

namespace MusicReplacer.ReplacementSystem;

public class MusicReplacementData
{
    private const float SoundMultiplierDefaultVolume = 5.3f;
    
    private Dictionary<string, string> ReplacementPaths { get; } = new();

    public void LoadFromSoundPack(SoundPack pack)
    {
        if (pack.SoundReplacements == null)
        {
            Plugin.Logger.LogWarning("Loading MusicReplacementData from empty sound pack");
            return;
        }
        
        foreach (var replacement in pack.SoundReplacements)
        {
            ReplacementPaths.Add(replacement.OriginalFileName, replacement.ReplacementFilePath);
        }
    }

    public SoundPack ConvertToSoundPack(string soundPackName)
    {
        var replacements = new List<SoundReplacement>();
        
        foreach (var replacement in ReplacementPaths)
        {
            replacements.Add(new SoundReplacement(replacement.Key, replacement.Value.Replace('\\', '/'), SoundMultiplierDefaultVolume));
        }

        var soundPack = new SoundPack
        {
            PackName = soundPackName,
            Enable = true,
            SoundReplacements = replacements.ToArray()
        };
        
        return soundPack;
    }
    
    public void SetReplacement(MusicSound sound, string newPath)
    {
        ReplacementPaths[sound.FileName] = newPath;
    }

    public bool TryGetCustomSound(MusicSound sound, out string customSoundPath)
    {
        return ReplacementPaths.TryGetValue(sound.FileName, out customSoundPath);
    }

    public bool SoundHasReplacement(MusicSound sound)
    {
        return ReplacementPaths.ContainsKey(sound.FileName);
    }

    public void SetSoundToDefault(MusicSound sound)
    {
        ReplacementPaths.Remove(sound.FileName);
    }
}