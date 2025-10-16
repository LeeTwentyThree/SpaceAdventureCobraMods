using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CobraSoundReplacer.Core;

public class CustomEClip
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("sound_name")]
    public string SoundName { get; set; }
}