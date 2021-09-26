using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private new AudioSource audio;
    [SerializeField] private AudioClip move;
    [SerializeField] private AudioClip capture;

    [HideInInspector] public static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PlayMoveSound()
    {
        audio.PlayOneShot(move);
    }

    public void PlayCaptureSound()
    {
        audio.PlayOneShot(capture);
    }
}
