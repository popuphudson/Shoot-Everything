using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Sound {
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _soundPrefab, _sound3DPrefab;
    [SerializeField] private AudioListener _listener;
    public AudioSource PlaySound(Sound __sound) {
        GameObject sound = Instantiate(_soundPrefab, _listener.transform);
        AudioSource source = sound.GetComponent<AudioSource>();
        source.clip = __sound.Clip;
        source.volume = __sound.Volume;
        Destroy(sound, __sound.Clip.length+0.5f);
        source.Play();
        return source;
    }

    public AudioSource PlaySoundAtPoint(Sound __sound, Vector3 __position) {
        GameObject sound = Instantiate(_sound3DPrefab, __position, Quaternion.identity);
        AudioSource source = sound.GetComponent<AudioSource>();
        source.clip = __sound.Clip;
        source.volume = __sound.Volume;
        Destroy(sound, __sound.Clip.length+0.5f);
        source.Play();
        return source;
    }
}
