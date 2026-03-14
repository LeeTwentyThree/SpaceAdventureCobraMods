using System;

namespace MusicReplacer.ReplacementSystem;

[Serializable]
public class MusicSound
{
    public MusicSound(int id, string fileName, string displayName)
    {
        Id = id;
        FileName = fileName;
        DisplayName = displayName;
    }
    
    public int Id { get; }
    public string DisplayName { get; }
    public string FileName { get; }
    public audioSelectionData.eCLIP EClip { get; set; }
}