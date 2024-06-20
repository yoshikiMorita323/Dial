using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // AudioSourceのボリュームをフェードアウトさせます。(対象のAudioSource,対象の現在のボリューム ,ボリュームが０になるまでの時間)
    public void AudioSourceFadeOutManager(AudioSource audioSource, float AudioVolume, float FadeOutTime)
    {
        audioSource.volume -= Time.deltaTime * (AudioVolume / FadeOutTime);
        if (audioSource.volume <= 0)
        {
            audioSource.Stop();
        }
    }

}
