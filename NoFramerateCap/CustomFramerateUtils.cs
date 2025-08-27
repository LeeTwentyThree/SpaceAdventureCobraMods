namespace NoFramerateCap;

public static class CustomFramerateUtils
{
    public static int GetNewFramerateInt() => Plugin.FrameRate.Value;
    
    public static float GetNewFramerateFloat() => Plugin.FrameRate.Value;
}