namespace MusicReplacer.ReplacementSystem;

public static class MusicPreviewUtils
{
    public static MusicPreview PreviewCurrentMusic(MusicSound music)
    {
        var sound = AudioController.Instance.PlaySound(music.EClip);
        var preview = new MusicPreview(sound);
        return preview;
    }
    
    public class MusicPreview
    {
        public MusicPreview(CAudio.CPlayingAudioData playingAudio)
        {
            _playingAudio = playingAudio;
        }

        private CAudio.CPlayingAudioData _playingAudio;
        
        public void StopPreview()
        {
            if (_playingAudio != null && _playingAudio.asrc != null)
            {
                _playingAudio.asrc.Stop();
                _playingAudio = null;
            }
        }
    }
}