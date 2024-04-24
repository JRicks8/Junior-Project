using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public enum Sound
    {
        PRODOTYPE_HER_music_TG,
        PRODOTYPE_HUB_music_TG,
        Colliding_Sound2,
        Colliding_Sound3,
        Hermes_Falling_Tyler,
        sfx_HarpyScreech1,
        sfx_BackPack_rolling,
        sfx_Backpack_walking,
        sfx_Chimes_and_Bells,
        sfx_CoinPickup,
        sfx_CollidingDelay_Sound1,
        sfx_Diving_Down,
        sfx_Harpy_Screech2,
        sfx_Skating_both_ears,
        sfx_Skating_separate_ears,
        sfx_SoulAmbience,
        sfx_UInoise,
        sfx_Walking_Leaves1,
        sfx_Walking_Leaves2,
        sfx_Walking_Leaves3,
        Skating_On_Coins,
        Walking_On_Coins,
        Walking_On_Concrete1,
        Walking_On_Concrete2,
        Walking_On_Grass1,
        Walking_On_Grass2,
        Walking_On_Wood,
        VO_DionysusDialogue1,
        VO_DionysusDialogue2,
        VO_DionysusDialogue3,
        VO_DionysusDialogue4,
        VO_DionysusDialogue5,
        VO_DionysusDialogue6,
        sfx_Lightning1,
        sfx_Lightning2,
        sfx_Lightning3,
        sfx_Thunder1,
        sfx_Thunder2,
        sfx_Thunder3,
        sfx_Thunder4,
        VO_Zeus1,
        VO_Zeus2,
        VO_Zeus3,
        VO_Zeus4,
        VO_Zeus5,
        VO_Zeus6,
        VO_Zeus7
    }

    public static MusicPlayer instance;

    public AudioClip[] audioClips;
    public List<AudioSource> tracks;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Two music players found in the same scene.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        for (int i = 0; i < audioClips.Length; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[i];
            audioSource.playOnAwake = false;

            tracks.Add(audioSource);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(Sound sound, bool loop, float volume = 1.0f)
    {
        tracks[(int)sound].loop = loop;
        tracks[(int)sound].volume = volume;
        tracks[(int)sound].Play();
    }

    public void PlaySoundOneShot(Sound sound, float volumeScale = 1.0f)
    {
        tracks[(int)sound].PlayOneShot(audioClips[(int)sound], volumeScale);
    }

    public void StopSound(Sound sound)
    {
        tracks[(int)sound].Stop();
    }

    public void PlaySoundFadeIn(Sound sound, float fadeInTime, bool loop, float targetVolume = 1.0f)
    {
        tracks[(int)sound].loop = loop;
        StartCoroutine(SoundFadeInHandler(sound, fadeInTime, targetVolume));
    }

    private IEnumerator SoundFadeInHandler(Sound sound, float fadeInTime, float targetVolume)
    {
        tracks[(int)sound].volume = 0.0f;
        tracks[(int)sound].Play();

        float timer = 0.0f;
        while (timer < fadeInTime)
        {
            timer += Time.fixedDeltaTime;

            tracks[(int)sound].volume = timer / fadeInTime * targetVolume;

            yield return new WaitForFixedUpdate();
        }

        tracks[(int)sound].volume = targetVolume;
    }

    public void StopSoundFadeOut(Sound sound, float fadeOutTime, float startVolume = 1.0f)
    {
        StartCoroutine(SoundFadeOutHandler(sound, fadeOutTime, startVolume));
    }

    private IEnumerator SoundFadeOutHandler(Sound sound, float fadeOutTime, float startVolume)
    {
        tracks[(int)sound].volume = startVolume;

        float timer = 0.0f;
        while (timer < fadeOutTime)
        {
            timer += Time.fixedDeltaTime;

            tracks[(int)sound].volume = (1 - timer / fadeOutTime) * startVolume;

            yield return new WaitForFixedUpdate();
        }

        tracks[(int)sound].Stop();
    }
}
