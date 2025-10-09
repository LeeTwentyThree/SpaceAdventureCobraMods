using System.Collections.Generic;
using System.Text;

namespace CobraSoundReplacer.Core;

public class SoundDatabase(CAudio cAudio)
{
    private Dictionary<string, ushort> _indexFromFileName;
    private Dictionary<ushort, string> _fileNameFromIndex;

    private ushort _nextFreeIndex;

    public void InitializeDatabase()
    {
        var allClips = cAudio.AllClip;

        _indexFromFileName = new Dictionary<string, ushort>();
        _fileNameFromIndex = new Dictionary<ushort, string>();
        for (ushort i = 0; i < allClips.Length; i++)
        {
            _indexFromFileName.Add(allClips[i].loadname, i);
            _fileNameFromIndex.Add(i, allClips[i].loadname);
        }

        _nextFreeIndex = (ushort)allClips.Length;
    }

    public bool TryGetFileNameFromIndex(ushort index, out string fileName)
    {
        return _fileNameFromIndex.TryGetValue(index, out fileName);
    }

    public bool TryGetIndexFromFileName(string fileName, out ushort index)
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

    public ushort GetNewReservedIndex()
    {
        var freeIndex = _nextFreeIndex;
        _nextFreeIndex++;
        return freeIndex;
    }
}