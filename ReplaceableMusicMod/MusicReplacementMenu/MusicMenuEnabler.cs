using UnityEngine;

namespace MusicReplacer.MusicReplacementMenu;

public class MusicMenuEnabler : MonoBehaviour
{
    public static MusicMenuEnabler Main { get; private set; }

    public NUIMainMenu mainMenu;
    public GameObject mainMenuTab;
    public GameObject musicMenu;

    private void Awake()
    {
        Main = this;
    }

    public void SetMusicMenuActive(bool active)
    {
        musicMenu.SetActive(active);
        mainMenuTab.SetActive(!active);
        mainMenu.enabled = !active;
    }
}