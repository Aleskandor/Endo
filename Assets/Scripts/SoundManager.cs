using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    private List<Sound> playing, removing;
    private Queue<Delegate> queue;

    private delegate void Delegate();
    private Delegate tempDelegate;

    public static SoundManager instance;
    IEnumerator musicTransition;

    void Awake()
    {
        queue = new Queue<Delegate>();
        playing = new List<Sound>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.originalVolume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    void Start()
    {
        PlayScene(SceneManager.GetActiveScene().buildIndex);
    }

    //transitionTime is a factor of 10 of the seconds i.e 20 equals 2 seconds
    IEnumerator FadeOut(int transitionDuration)
    {
        if (removing.Count == 0)
        {
            StopCoroutine(musicTransition);
            if (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
            }
            musicTransition = null;
        }

        for (int i = 0; i < transitionDuration + 1; i++)
        {
            foreach (Sound s in removing)
            {
                s.source.volume *= (transitionDuration - i) * (1f / transitionDuration);

                if (s.source.volume <= 0)
                {
                    s.source.Stop();
                }
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        foreach (Sound s in removing)
        {
            if (s.source.volume <= 0)
            {
                s.source.Stop();
            }
        }

        removing.RemoveAll(sound => sound.volume <= 0);

        StopCoroutine(musicTransition);
        if (queue.Count > 0)
        {
            playing.Clear();
            queue.Dequeue().Invoke();
        }

        musicTransition = null;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found.");
            return;
        }
        if (name != "Transition")
        {
            playing.Add(s);
        }
        s.source.volume = s.originalVolume;
        s.source.Play();
    }

    public void PlayScene(int index)
    {
        if (index == 0)
        {
            PlayTitleScene();
        }
        if (index == 1)
        {
            PlayCampScene();
        }
        else if (index == 2)
        {
            PlayEndlessForestScene();
        }
        else if (index == 3)
        {
            PlayRiverScene();
        }
        else if (index == 4)
        {
            PlayGroveScene();
        }
    }

    private void Update()
    {
        playing.RemoveAll(sound => sound.source.isPlaying == false);
    }

    public void PlayWhistle()
    {
        if (playing.Find(sound => sound.name == "Whistle") == null)
        {
            Play("Whistle");
        }
    }

    private void PlayTitleScene()
    {
        queue.Clear();
        tempDelegate = new Delegate(PlayTitle);
        queue.Enqueue(tempDelegate);
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }
        removing = playing;
        musicTransition = FadeOut(20);
        StartCoroutine(musicTransition);
    }

    private void PlayTitle()
    {
        Play("Chill");
        Play("Birds");
    }

    private void PlayEndlessForestScene()
    {
        queue.Clear();
        tempDelegate = new Delegate(PlayEndlessForest);
        queue.Enqueue(tempDelegate);

        removing = playing;
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }
        musicTransition = FadeOut(20);
        StartCoroutine(musicTransition);
    }

    private void PlayEndlessForest()
    {
        Play("EndlessForest");
        Play("Owls");
    }

    private void PlayRiverScene()
    {
        queue.Clear();
        tempDelegate = new Delegate(PlayRiver);
        queue.Enqueue(tempDelegate);
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }
        removing = playing;
        musicTransition = FadeOut(20);
        StartCoroutine(musicTransition);
    }

    private void PlayRiver()
    {
        Play("MainTheme");
        Play("Birds");
    }

    private void PlayGroveScene()
    {
        queue.Clear();
        tempDelegate = new Delegate(PlayGrove);
        queue.Enqueue(tempDelegate);
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }
        removing = playing;
        musicTransition = FadeOut(20);
        StartCoroutine(musicTransition);
    }

    private void PlayGrove()
    {
        Play("Mellow");
    }

    private void PlayCampScene()
    {
        queue.Clear();
        tempDelegate = new Delegate(PlayCamp);
        queue.Enqueue(tempDelegate);
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }
        removing = playing;
        musicTransition = FadeOut(20);
        StartCoroutine(musicTransition);
    }

    private void PlayCamp()
    {
        Play("MainTheme");
        Play("Birds");
    }
}

