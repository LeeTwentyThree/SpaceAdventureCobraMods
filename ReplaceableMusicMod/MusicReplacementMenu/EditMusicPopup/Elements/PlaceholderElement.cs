namespace MusicReplacer.MusicReplacementMenu.EditMusicPopup.Elements;

public class PlaceholderElement : MusicEditorElementBase
{
    public static PlaceholderElement Create()
    {
        return CreateBase().gameObject.AddComponent<PlaceholderElement>();
    }
}