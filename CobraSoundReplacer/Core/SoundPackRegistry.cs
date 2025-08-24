using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using CobraSoundReplacer.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace CobraSoundReplacer.Core;

public static class SoundPackRegistry
{
    private static readonly Dictionary<string, RegisteredSoundPack> SoundPacks = [];

    private static SoundDatabase _soundDatabase;

    private static CAudio _cAudio;

    internal static readonly Dictionary<ushort, AudioClip> SoundReplacements = new();
    internal static readonly Dictionary<ushort, float> CustomSoundVolumes = new();

    public static IEnumerator RegisterSoundPack(SoundPack pack, string containingFolderPath)
    {
        if (SoundPacks.ContainsKey(pack.PackName))
        {
            Plugin.Logger.LogWarning($"A sound pack is already registered by the name of '{pack.PackName}'. Skipping.");
            yield break;
        }

        var registered = new RegisteredSoundPack(pack, containingFolderPath);
        SoundPacks.Add(pack.PackName, registered);
        yield return RefreshPack(registered);
    }

    internal static IEnumerator LoadPacksAutomatically()
    {
        var pluginFolder = Paths.PluginPath;
        var allFiles = Directory.GetFiles(pluginFolder, "*." + Constants.FileExtension, SearchOption.AllDirectories);
        if (allFiles.Length == 0)
        {
            Plugin.Logger.LogWarning($"No packs found in plugin folder. Skipping.");
            yield break;
        }

        Plugin.Logger.LogInfo($"Found {allFiles.Length} sound packs to load. Loading...");

        int successes = 0;
        foreach (var path in allFiles)
        {
            SoundPack pack = null;
            try
            {
                var text = File.ReadAllText(path);
                pack = JsonConvert.DeserializeObject<SoundPack>(text);
                successes++;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Exception thrown while loading sound pack at path '{path}': " + e.Message);
                continue;
            }

            yield return RegisterSoundPack(pack, Path.GetDirectoryName(path));
        }

        Plugin.Logger.LogInfo($"Loaded {successes} sound pack(s).");
    }

    internal static IEnumerator OnAudioInitialized(CAudio cAudio)
    {
        _cAudio = cAudio;
        _soundDatabase = new SoundDatabase(_cAudio);
        _soundDatabase.InitializeDatabase();

        foreach (var pack in SoundPacks)
        {
            yield return RefreshPack(pack.Value);
        }
    }

    private static IEnumerator RefreshPack(RegisteredSoundPack pack)
    {
        if (_cAudio == null)
        {
            yield break;
        }

        if (!pack.InitializedEver)
        {
            yield return InitializeSoundPack(pack);
        }
    }

    private static IEnumerator InitializeSoundPack(RegisteredSoundPack pack)
    {
        foreach (var replacement in pack.Pack.SoundReplacements)
        {
            yield return SetSoundReplacement(replacement.OriginalFileName,
                Path.Combine(pack.ContainingFolderPath, replacement.ReplacementFilePath), replacement.Volume);
        }

        pack.InitializedEver = true;
    }

    private static IEnumerator SetSoundReplacement(string originalFileName, string replacementFilePath, float volume)
    {
        if (!_soundDatabase.TryGetIndexFromFileName(originalFileName, out var index))
        {
            Plugin.Logger.LogWarning($"Failed to find original clip by name '{originalFileName}'. Skipping.");
            yield break;
        }

        var result = new TaskResult<AudioClip>();
        var loadedClip = AudioLoadingUtils.LoadAudioClipFromPath(replacementFilePath, result);
        yield return loadedClip;
        if (!result.Success)
        {
            Plugin.Logger.LogWarning($"Failed to load sound file at path '{replacementFilePath}'.");
            yield break;
        }

        var clip = result.GetResult();
        if (clip == null)
        {
            Plugin.Logger.LogWarning($"Clip from path '{replacementFilePath}' is nulL!.");
            yield break;
        }
        
        SoundReplacements[index] = clip;
        CustomSoundVolumes[index] = volume;
    }

    private class RegisteredSoundPack
    {
        public SoundPack Pack { get; }
        public string ContainingFolderPath { get; }
        public bool InitializedEver { get; set; }

        public RegisteredSoundPack(SoundPack pack, string containingFolderPath)
        {
            Pack = pack;
            ContainingFolderPath = containingFolderPath;
        }
    }
}