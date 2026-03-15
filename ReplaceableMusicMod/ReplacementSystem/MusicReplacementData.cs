using System;
using System.Collections.Generic;
using CobraSoundReplacer.Core;

namespace MusicReplacer.ReplacementSystem;

public class MusicReplacementData
{
    private const float SoundMultiplierDefaultVolume = 5.3f;
    
    private Dictionary<string, string> ReplacementPaths { get; } = new();
    private Dictionary<string, float> CustomVolumes { get; } = new();
    private HashSet<string> PlayableCustomSounds { get; } = new(StringComparer.OrdinalIgnoreCase);

    public void LoadFromSoundPack(SoundPack pack)
    {
        if (pack.SoundReplacements == null)
        {
            Plugin.Logger.LogWarning("Loading MusicReplacementData from empty sound pack");
            return;
        }

        if (ReplacementPaths.Count != 0 || CustomVolumes.Count != 0)
        {
            Plugin.Logger.LogWarning("Loading MusicReplacementData from a pack while data already exists");
        }
        
        foreach (var replacement in pack.SoundReplacements)
        {
            ReplacementPaths.Add(replacement.OriginalFileName, replacement.ReplacementFilePath);
            CustomVolumes.Add(replacement.OriginalFileName, replacement.Volume / SoundMultiplierDefaultVolume);
            PlayableCustomSounds.Add(FormatPlayableSound(replacement.OriginalFileName, replacement.ReplacementFilePath));
        }
    }

    public SoundPack ConvertToSoundPack(string soundPackName)
    {
        var replacements = new List<SoundReplacement>();
        
        foreach (var replacement in ReplacementPaths)
        {
            var volume = CustomVolumes.GetValueOrDefault(replacement.Key, 1) * SoundMultiplierDefaultVolume;
            replacements.Add(new SoundReplacement(replacement.Key, replacement.Value.Replace('\\', '/'), volume));
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
        return TryGetCustomSound(sound.FileName, out customSoundPath);
    }
    
    public bool TryGetCustomSound(string soundFileName, out string customSoundPath)
    {
        return ReplacementPaths.TryGetValue(soundFileName, out customSoundPath);
    }

    public void SetSoundVolume(MusicSound sound, float volume)
    {
        CustomVolumes[sound.FileName] = volume;
    }

    public float GetSoundVolume(MusicSound sound)
    {
        return GetSoundVolume(sound.FileName);
    }
    
    public float GetSoundVolume(string soundFileName)
    {
        return CustomVolumes.GetValueOrDefault(soundFileName, 1);
    }

    public bool SoundHasReplacement(MusicSound sound)
    {
        return ReplacementPaths.ContainsKey(sound.FileName);
    }
    
    public bool SoundHasReplacement(string soundFileName)
    {
        return ReplacementPaths.ContainsKey(soundFileName);
    }

    public void SetSoundToDefault(MusicSound sound)
    {
        ReplacementPaths.Remove(sound.FileName);
        CustomVolumes.Remove(sound.FileName);
    }

    public bool CanPreviewCustomSound(MusicSound sound)
    {
        if (!TryGetCustomSound(sound, out string customSoundPath))
            return true;
        return PlayableCustomSounds.Contains(FormatPlayableSound(sound.FileName, customSoundPath));
    }

    private static string FormatPlayableSound(string fileName, string customSoundPath)
    {
        return fileName + ":" + customSoundPath;
    }
}