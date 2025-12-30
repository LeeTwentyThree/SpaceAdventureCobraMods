using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace IntroSwapMod;

public class VideoDatabase
{
    private readonly string _folderPath;
    private readonly Dictionary<string, VideoClip> _bundleVideos = new();
    private readonly Dictionary<string, AssetBundle> _bundles = new();

    public VideoDatabase(string folderPath)
    {
        _folderPath = folderPath;
    }

    public bool TrySelectVideo(string bundleName, out VideoClip clip)
    {
        if (_bundleVideos.TryGetValue(bundleName, out var bundleVideo))
        {
            clip = bundleVideo;
            return true;
        }

        var path = GetBundlePath(bundleName);
        if (!File.Exists(path))
        {
            clip = null;
            Plugin.Logger.LogWarning("Failed to find bundle at path: " + path);
            return false;
        }

        if (!GetBundleAtPath(path, out var bundle))
        {
            clip = null;
            return false;
        }

        clip = bundle.LoadAllAssets<VideoClip>().FirstOrDefault();

        if (clip == null)
        {
            Plugin.Logger.LogWarning($"Failed to find a clip in bundle '{bundleName}'");
            return false;
        }
        
        _bundleVideos.Add(bundleName, clip);
        return true;
    }

    private bool GetBundleAtPath(string path, out AssetBundle bundle)
    {
        bundle = null;
        if (_bundles.TryGetValue(path, out bundle))
        {
            return true;
        }
        if (!File.Exists(path))
        {
            return false;
        }
        bundle = AssetBundle.LoadFromFile(path);
        _bundles.Add(path, bundle);
        return bundle != null;
    }

    private string GetBundlePath(string bundleName)
    {
        return Path.Combine(_folderPath, bundleName);
    }
}