using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class handles the events of audio source
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    //public AudioSource[] audioSlot;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;
        }
    }

    public void ResetPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        float originalVol = s.source.volume;
        s.source.volume = 0f;
        s.source.Play();
        StartCoroutine(ResetVolume(s, s.source.volume, originalVol));
    }

    IEnumerator ResetVolume(Sound s, float volume, float originalVol)
    {
        while (s.source.volume <= originalVol)
        {
            s.source.volume += Time.deltaTime;
            yield return null;
        }
    }

    public void Play(string name, bool isOneShot, float startTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.time = startTime;
        if (isOneShot)
        {
            s.source.PlayOneShot(s.clip);
        }
        else
        {
            s.source.Play();
        }
    }

    public void PlayDelayed(string name, float delay)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.PlayDelayed(delay);
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    public void SetTime(string name, float time)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.time = time;
    }

    public float? GetTime(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return null;
        }
        else
        {
            return s.source.time;
        }
    }

    //fade in or fade out volume within duration
    public void SetAndFade(string name, float duration, float startVolume, float targetVolume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
        }
        else
        {
            StartCoroutine(StartFade(s.source, duration, startVolume, targetVolume));
        }
    }

    private IEnumerator StartFade(AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        float currentTime = 0;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public Sound GetAudioSource(string _name)
    {
        return Array.Find(sounds, sound => sound.name == _name);
    }
}
