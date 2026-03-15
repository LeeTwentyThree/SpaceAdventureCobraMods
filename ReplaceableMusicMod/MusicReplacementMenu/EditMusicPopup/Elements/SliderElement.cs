using System;
using UnityEngine;
using UnityEngine.UI;

namespace MusicReplacer.MusicReplacementMenu.EditMusicPopup.Elements;

public class SliderElement : MusicEditorElementBase, ISelectableElement
{
    private static readonly Color NormalBackgroundColor = new(0.6f, 0.3f, 0.3f, 0.3f);
    private static readonly Color HighlightedBackgroundColor = new(0.9f, 0.2f, 0.2f, 0.5f);
    private static readonly Color NormalSliderColor = new(0.9f, 0.2f, 0.2f, 0.5f);
    private static readonly Color HighlightedSliderColor = new(1, 0, 0, 0.5f);

    private static readonly Color NormalTextColor = new(1, 1, 1);

    private Image _backgroundImage;
    private Image _sliderImage;
    private Text _valueText;

    private Action<float> _onUse;
    private Func<float, string> _formatText;

    private float _sliderValue; // normalized
    private float _sliderStep;

    private bool _selected;

    public static SliderElement Create(string labelText, Action<float> onUse, float defaultValue, float step,
        Func<float, string> formatText, int fontSize = 80)
    {
        var element = CreateBase(); // top-level empty container

        // Label
        var labelTransform = AddChild("Label", element);
        var label = labelTransform.gameObject.AddComponent<Text>();
        if (LoadSaveController.Instance.PreferencesData.language == TextsController.LANGUAGE.JAPANESE)
            fontSize = Mathf.RoundToInt(fontSize * 0.7f);
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleLeft;
        label.font = MusicMenuBuilder.ButtonFont;
        label.color = NormalTextColor;
        label.text = labelText;
        labelTransform.anchorMin = new Vector2(0, 0);
        labelTransform.anchorMax = new Vector2(0.4f, 1);
        labelTransform.offsetMin = new Vector2(50, 0);
        labelTransform.offsetMax = new Vector2(-50, 0);

        // Slider
        var containerTransform = AddChild("SliderContainer", element);
        containerTransform.anchorMin = new Vector2(0.4f, 0);
        containerTransform.anchorMax = new Vector2(1, 1);
        containerTransform.offsetMin = Vector2.zero;
        containerTransform.offsetMax = Vector2.zero;
        var backgroundImage = containerTransform.gameObject.AddComponent<Image>();
        backgroundImage.color = NormalBackgroundColor;

        // Slider bar
        var sliderTransform = AddChild("Slider", containerTransform);
        var sliderImage = sliderTransform.gameObject.AddComponent<Image>();
        sliderImage.color = NormalSliderColor;
        sliderTransform.anchorMin = new Vector2(0, 0f);
        sliderTransform.anchorMax = new Vector2(1f, 1f);
        sliderTransform.localPosition = Vector3.zero;
        sliderTransform.pivot = new Vector2(0, 0.5f);
        sliderTransform.offsetMin = new Vector2(20, 20);
        sliderTransform.offsetMax = new Vector2(-20, -20);

        // Value
        var valueTextTransform = AddChild("ValueText", containerTransform);
        var valueText = valueTextTransform.gameObject.AddComponent<Text>();
        valueText.fontSize = 70;
        valueText.alignment = TextAnchor.MiddleRight;
        valueText.font = MusicMenuBuilder.ButtonFont;
        valueText.color = NormalTextColor;
        valueTextTransform.anchorMin = new Vector2(0.8f, 0);
        valueTextTransform.anchorMax = new Vector2(1f, 1);
        valueTextTransform.offsetMin = new Vector2(0, 0);
        valueTextTransform.offsetMax = new Vector2(-10, 0);

        // Behaviour
        var sliderElement = element.gameObject.AddComponent<SliderElement>();
        sliderElement._sliderImage = sliderImage;
        sliderElement._valueText = valueText;
        sliderElement._backgroundImage = backgroundImage;
        sliderElement._onUse = onUse;
        sliderElement._formatText = formatText;
        sliderElement._sliderStep = step;
        sliderElement._sliderValue = defaultValue;

        // Initialize
        sliderElement.UpdateSliderVisual();

        return sliderElement;
    }

    public void Select()
    {
        _backgroundImage.color = HighlightedBackgroundColor;
        _sliderImage.color = HighlightedSliderColor;
        _selected = true;
    }

    public void Deselect()
    {
        _backgroundImage.color = NormalBackgroundColor;
        _sliderImage.color = NormalSliderColor;
        _selected = false;
    }

    public void Interact()
    {
    }

    private void Update()
    {
        if (!_selected) return;
        bool changed = false;

        if (Utils.GetButtonPushed(PadsController.LS_LEFT))
        {
            _sliderValue -= _sliderStep;
            changed = true;
        }
        else if (Utils.GetButtonPushed(PadsController.LS_RIGHT))
        {
            _sliderValue += _sliderStep;
            changed = true;
        }

        if (changed)
        {
            _sliderValue = Mathf.Clamp01(_sliderValue);
            UpdateSliderVisual();
            _onUse?.Invoke(_sliderValue); // call callback with updated value
        }
    }

    private void UpdateSliderVisual()
    {
        if (_sliderImage != null)
        {
            // Scale X according to slider value (0 = empty, 1 = full)
            _sliderImage.rectTransform.localScale = new Vector3(_sliderValue, 1, 1);
        }

        if (_valueText != null && _formatText != null)
        {
            _valueText.text = _formatText.Invoke(_sliderValue);
        }
    }
}