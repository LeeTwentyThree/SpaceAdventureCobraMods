namespace MusicReplacer;

public static class Utils
{
    public static bool GetClickingOnUI(bool bypassPause = true)
    {
        PadsController.INPUTFILTER inputFilter = PadsController.INPUTFILTER.CAT_INGAME;
        if (bypassPause)
            inputFilter |= PadsController.INPUTFILTER.OPT_PAUSE;
        var button = PadsController.Instance.TrigUI(-1, inputFilter);
        return (button & PadsController.BUTTON_A) != 0;
    }
    
    public static bool GetButtonPushed(int requiredButton, bool bypassPause = true)
    {
        PadsController.INPUTFILTER inputFilter = PadsController.INPUTFILTER.CAT_INGAME;
        if (bypassPause)
            inputFilter |= PadsController.INPUTFILTER.OPT_PAUSE;
        var button = PadsController.Instance.TrigUI(-1, inputFilter);
        return (button & requiredButton) != 0;
    }
}