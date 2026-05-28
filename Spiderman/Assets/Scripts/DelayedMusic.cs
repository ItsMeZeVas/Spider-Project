using UnityEngine;
using System.Collections;

public class DelayedMusic : MonoBehaviour
{
    public AudioSource musicSource;
    public float delay = 4f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        musicSource.Play();
    }
}