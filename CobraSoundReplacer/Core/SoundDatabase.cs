using System.Collections.Generic;
using System.Text;

namespace CobraSoundReplacer.Core;

public class SoundDatabase(CAudio cAudio)
{
    private Dictionary<string, int> _indexFromFileName;
    private Dictionary<int, string> _fileNameFromIndex;

    public void InitializeDatabase()
    {
        var allClips = cAudio.AllClip;

        _indexFromFileName = new Dictionary<string, int>();
        _fileNameFromIndex = new Dictionary<int, string>();
        for (int i = 0; i < allClips.Length; i++)
        {
            _indexFromFileName.Add(allClips[i].loadname, i);
            _fileNameFromIndex.Add(i, allClips[i].loadname);
        }
    }

    public bool TryGetFileNameFromIndex(int index, out string fileName)
    {
        return _fileNameFromIndex.TryGetValue(index, out fileName);
    }

    public bool TryGetIndexFromFileName(string fileName, out int index)
    {
        return _indexFromFileName.TryGetValue(fileName, out index);
    }

    public void SaveToFile(string path)
    {
        var sb = new StringBuilder();
        foreach (var entry in _fileNameFromIndex)
        {
            sb.Append($"{entry.Key.ToString(),-5}: {entry.Value}\n");
        }
        System.IO.File.WriteAllText(path, sb.ToString());
    }
}