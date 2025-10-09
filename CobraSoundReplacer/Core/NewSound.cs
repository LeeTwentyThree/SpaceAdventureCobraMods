using Newtonsoft.Json;

namespace CobraSoundReplacer.Core;

public class NewSound
{
    [JsonProperty("clip_name")]
    public string ClipName { get; set; }

    [JsonProperty("file_path")]
    public string FilePath { get; set; }

    [JsonProperty("volume")]
    public float Volume { get; set; } = 1f;
    
    [JsonProperty("volume_type")]
    public sbyte VolumeType { get; set; }
    
    [JsonProperty("looping")]
    public bool Looping { get; set; }

    [JsonConstructor]
    public NewSound()
    {
    }

    public NewSound(string clipName, string filePath, float volume)
    {
        ClipName = clipName;
        FilePath = filePath;
        Volume = volume;
    }
}