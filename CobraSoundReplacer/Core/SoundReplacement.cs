using Newtonsoft.Json;

namespace CobraSoundReplacer.Core;

public class SoundReplacement
{
    [JsonProperty("original_file_name")]
    public string OriginalFileName { get; set; }

    [JsonProperty("replacement_file_path")]
    public string ReplacementFilePath { get; set; }

    [JsonProperty("volume")]
    public float Volume { get; set; } = 1f;

    [JsonConstructor]
    public SoundReplacement()
    {
    }

    public SoundReplacement(string originalFileName, string replacementFilePath, float volume = 1f)
    {
        OriginalFileName = originalFileName;
        ReplacementFilePath = replacementFilePath;
        Volume = volume;
    }
}