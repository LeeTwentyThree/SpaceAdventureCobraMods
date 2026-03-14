using MusicReplacer.ReplacementSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MusicReplacer.MusicReplacementMenu;

public class MusicButton : MonoBehaviour
{
    private Color _normalColor;
    public MusicSound data;
    public Color selectedColor;
    public Image image;

    private bool _normalColorSet;
    
    public void SetStateSelected(bool selected)
    {
        if (!_normalColorSet)
        {
            _normalColor = image.color;
            _normalColorSet = true;
        }
        image.color = selected ? selectedColor : _normalColor;
    }
}