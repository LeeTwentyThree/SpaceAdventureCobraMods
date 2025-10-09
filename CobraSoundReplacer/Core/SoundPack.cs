using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CobraSoundReplacer.Core;

public class SoundPack
{
    [JsonProperty("pack_name")]
    public string PackName { get; set; }
    
    [JsonProperty("enable")]
    public bool Enable { get; set; }
    
    [JsonProperty("sound_replacements")]
    [CanBeNull]
    public SoundReplacement[] SoundReplacements { get; set; }

    [JsonProperty("new_sounds")]
    [CanBeNull]
    public NewSound[] NewSounds { get; set; }

    [JsonConstructor]
    public SoundPack()
    {
        
    }
    
    public SoundPack(SoundReplacement[] replacements)
    {
        SoundReplacements = replacements;
    }
}