using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioController : MonoBehaviour {


    public AudioClip MusicClip;
    public Slider volumeSlider;

    // the component that Unity uses to play your clip
    public AudioSource MusicSource;

    // Use this for initialization
    void Start()
    {
        MusicSource.clip = MusicClip;
        MusicSource.volume = volumeSlider.value;
        MusicSource.Play();
    }

    // Update is called once per frame

    public void mute()
    {

        MusicSource.mute = !MusicSource.mute;

    }


    public void VolumeController()
    {
        MusicSource.volume = volumeSlider.value;

    }
}