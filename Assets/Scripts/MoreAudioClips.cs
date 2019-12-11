using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
 public class MoreAudioClips : MonoBehaviour
{
    AudioSource ac = null;

    private int previousRandom = -1;
    /// <summary>
    /// MoreAudioClips: stores a list of audio clips so that they can be played from an audio source on the gameobject
    /// </summary>
    /// Attached to: a gameobject that has an audio source and you want to use multiple audio clips for
    public List<AudioClip> clips; // store the audio clips
    public List<float> volumes;

    private void Awake()
    {
        ac = GetComponent<AudioSource>();
    }

    void Start()
    {        
        while (volumes.Count < clips.Count) //ensure volumes are valid for the clip indices
        {
            volumes.Add(1f); // set a default volume of 1 for clips with no specified volume
        }
    }


    public void PlayClip(int clipNum)
    { // used to play a specific clip from another script
      // GetComponent<MoreAudioClips>().PlayClip[2] would play the 3rd clip (index 2) that you set in the Inspector list for MoreAudioCLips
        { // used to play a random clip from the set of clips
            if (ac == null)
            {
                ac = GetComponent<AudioSource>();
            }
            if (clips.Count > 0 && clipNum >= 0 && clipNum < clips.Count)
                ac.PlayOneShot(clips[clipNum], volumes[clipNum]); //uses the AudioSource on the current gameObject
        }  
    }

    public void PlayRandomClip()
    { // used to play a random clip from the set of clips
        if (ac == null)
        {
            ac = GetComponent<AudioSource>();
        }
        if (clips.Count > 0)
        {
            float startPitch = ac.pitch;
            int clipNum = Random.Range(0, clips.Count - 1);
            //To prevent us from playing the same sound twice
            if (previousRandom == clipNum)
            {
                if (clipNum >= clips.Count-1)
                {
                    clipNum--;
                }
                else
                {
                    clipNum++;
                }
            }        
            previousRandom = clipNum;
            float volume = Random.Range(0.5f, volumes[clipNum]);
            ac.pitch = Random.Range(0.6f, 1.2f);
            ac.PlayOneShot(clips[clipNum], volume);
            ac.pitch = startPitch;
        }
    }

    public void PlayRandomClipDelayed(float delay)
    { // used to play a random clip from the set of clips

        if (clips.Count > 0)
        {
            float startPitch = ac.pitch;
            int clipNum = Random.Range(0, clips.Count - 1);
            //To prevent us from playing the same sound twice
            if (previousRandom == clipNum)
            {
                if (clipNum >= clips.Count - 1)
                {
                    clipNum--;
                }
                else
                {
                    clipNum++;
                }
            }

            ac.clip = clips[clipNum];
            previousRandom = clipNum;
            ac.volume = Random.Range(0.9f, volumes[clipNum]);
            ac.pitch = Random.Range(0.9f, 1.1f);
            ac.PlayDelayed(delay);
            //GetComponent<AudioSource>().PlayOneShot(clips[clipNum], volume);
            ac.pitch = startPitch;
        }
    }
}
