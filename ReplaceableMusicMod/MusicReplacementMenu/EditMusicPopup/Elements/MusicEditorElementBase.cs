using UnityEngine;

namespace MusicReplacer.MusicReplacementMenu.EditMusicPopup.Elements;

public abstract class MusicEditorElementBase : MonoBehaviour
{
    public RectTransform RectTransform { get; private set; }

    protected virtual void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        if (RectTransform == null)
            Plugin.Logger.LogWarning("RectTransform not found on " + gameObject);
    }

    protected static RectTransform CreateBase(float height = 230)
    {
        var rectTransform = new GameObject("Element").AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(800, height);
        return rectTransform;
    }

    protected static RectTransform AddChild(string childName, RectTransform parent)
    {
        var child = new GameObject(childName);
        var childRect = child.AddComponent<RectTransform>();
        childRect.SetParent(parent);
        childRect.localScale = Vector3.one;
        return childRect;
    }
}