using System.Collections.Generic;
using MusicReplacer.MusicReplacementMenu.EditMusicPopup.Elements;
using MusicReplacer.ReplacementSystem;
using UnityEngine;

namespace MusicReplacer.MusicReplacementMenu.EditMusicPopup;

public class MusicEditor : MonoBehaviour
{
    public RectTransform rect;
    public FileChooserMenu fileChooser;
    
    private readonly List<MusicEditorElementBase> _elements = [];
    private readonly List<ISelectableElement> _selectables = [];

    private int _mainChoice;
    private int _previousChoice;

    private MusicPreviewUtils.MusicPreview _preview;
    
    private void Update()
    {
        if (_selectables.Count == 0)
            return;

        if (fileChooser.IsOpen)
            return;
        
        UIController.HandleCursor(ref _mainChoice, _selectables.Count, 1, 2, _allowbuttonscycle: false, UIFooter.PREDEFINEDTYPE.GENERIC_VALIDATE, delegate
            {
                _selectables[_mainChoice].Interact();
            },
            HideWindow, OnChoiceChange, OnChoiceChange);

    }

    public void ShowWindow(MusicSound sound)
    {
        if (_elements.Count > 0)
            ClearWindow();
        
        gameObject.SetActive(true);
        
        AddElement(LabelElement.Create(sound.DisplayName, 130));
        AddElement(LabelElement.Create("Original file name: " + sound.FileName, 50, 60));
        AddElement(LabelElement.Create("Current sound: " + GetCurrentSoundName(sound)));
        if (AudioController.Audio.volume[2] <= 0.1f)
            AddElement(LabelElement.Create("<color=#FF0000>WARNING: Music volume is low or disabled!</color>"));
        AddElement(ButtonElement.Create("Preview current sound", () => PreviewMusic(sound, false)));
        AddElement(ButtonElement.Create("Preview original sound", () => PreviewMusic(sound, true)));
        AddElement(ButtonElement.Create("Replace this sound", () => fileChooser.Show(sound)));
        AddElement(ButtonElement.Create("Return & Save", HideWindow));

        _mainChoice = 0;
        _previousChoice = 0;
        if (_selectables.Count > 0)
        {
            _selectables[_mainChoice].Select();
        }
    }

    private void PreviewMusic(MusicSound sound, bool originalSound)
    {
        StopMusicPreview();
        _preview = MusicPreviewUtils.PreviewCurrentMusic(sound);
    }

    private string GetCurrentSoundName(MusicSound sound)
    {
        if (!MusicReplacementManager.ReplacementData.TryGetCustomSound(sound, out var replacement))
            return "DEFAULT";
        return FileManagement.GetDisplayNameForSoundPath(replacement);
    }

    private void StopMusicPreview()
    {
        _preview?.StopPreview();
        _preview = null;
    }

    public void HideWindow()
    {
        gameObject.SetActive(false);
        StopMusicPreview();
        MusicReplacementManager.SaveChanges();
    }

    public bool GetIsShown()
    {
        return isActiveAndEnabled;
    }

    private void OnChoiceChange()
    {
        if (_selectables.Count == 0)
            return;

        if (_previousChoice != _mainChoice && _mainChoice < _selectables.Count)
        {
            _selectables[_previousChoice].Deselect();
        }
        
        _selectables[_mainChoice].Select();
        _previousChoice = _mainChoice;
    }

    private void ClearWindow()
    {
        foreach (var element in _elements)
        {
            if (element == null)
            {
                Plugin.Logger.LogWarning("Element is null. This should not happen!");
                continue;
            }
            Destroy(element.gameObject);
        }
        _elements.Clear();
        _selectables.Clear();
    }

    private void AddElement(MusicEditorElementBase element)
    {
        element.RectTransform.SetParent(rect);
        element.RectTransform.localScale = Vector3.one;
        if (element is ISelectableElement selectable)
        {
            _selectables.Add(selectable);
        }
        _elements.Add(element);
    }
}