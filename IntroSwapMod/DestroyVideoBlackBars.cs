using UnityEngine;
using UnityEngine.Video;

namespace IntroSwapMod;

public class DestroyVideoBlackBars : MonoBehaviour
{
    public GameObject bars;
    public VideoPlayer player;
    public float startDelay = 1f;
    
    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;
    }
    
    private void Update()
    {
        if (Time.time < _startTime + startDelay)
        {
            return;
        }
        
        if (bars == null)
        {
            Destroy(gameObject);
            return;
        }

        if (player == null || !player.isPlaying)
        {
            Destroy(bars);
            Destroy(gameObject);
        }
    }
}