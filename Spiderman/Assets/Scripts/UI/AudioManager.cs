using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mixer;

    private void Awake()
    {
        Instance = this;
    }

    public void SetVolume(string parameter, float value)
    {
        mixer.SetFloat(parameter, value);
    }
}
