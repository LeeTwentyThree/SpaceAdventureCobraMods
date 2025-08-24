using Newtonsoft.Json;

namespace CobraSoundReplacer.Core;

public class SoundPack
{
    [JsonProperty("pack_name")]
    public string PackName { get; set; }
    
    [JsonProperty("enable")]
    public bool Enable { get; set; }
    
    [JsonProperty("sound_replacements")]
    public SoundReplacement[] SoundReplacements { get; set; }

    [JsonConstructor]
    public SoundPack()
    {
        
    }
    
    public SoundPack(SoundReplacement[] replacements)
    {
        SoundReplacements = replacements;
    }
}