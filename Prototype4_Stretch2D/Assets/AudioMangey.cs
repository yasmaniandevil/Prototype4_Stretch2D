using System.Collections;
using System.Collections.Generic;
using Dimension19.AudioEvents;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioMangey : MonoBehaviour
{
    public AudioEventsComponent audioEventsComponent;
    
    
    //public Button playButton;
    //public Button stopButton;

    private void Awake()
    {
        audioEventsComponent.audioSource.Play();
        Debug.Log("Name: " + audioEventsComponent.audioSource.name);
        //playButton.onClick.AddListener(PlayButton);
        //stopButton.onClick.AddListener(StopButton);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //SetPitch(audioEventsComponent.audioSource.pitch);
        //SetVolumeLevel(audioEventsComponent.audioSource.volume);
        //Debug.Log("Name: " + audioEventsComponent.audioSource.name);
        //Debug.Log("Volume Level" + audioEventsComponent.audioSource.volume);
        //SetTime(audioEventsComponent.audioSource.time);
        //SetDoppler(audioEventsComponent.audioSource.dopplerLevel);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPitch(float pitchValue)
    {
        audioEventsComponent.audioSource.pitch = pitchValue;
    }

    public void SetVolumeLevel(float sliderValue)
    {
        audioEventsComponent.audioSource.volume = sliderValue;
    }

    public void SetTime(float timeValue)
    {
         audioEventsComponent.audioSource.time = timeValue;
         Debug.Log("Time Value: " + audioEventsComponent.audioSource.time);
    }

    public void SetReverb(float reverbValue)
    {
        audioEventsComponent.audioSource.reverbZoneMix = reverbValue;
        Debug.Log("Reverb Value:" + audioEventsComponent.audioSource.reverbZoneMix);
    }
    
    // private void PlayButton()
    // {
    //     audioEventsComponent.audioSource.Play();
    //     Debug.Log("Play Clicked");
    // }
    //
    // private void StopButton()
    // {
    //     audioEventsComponent.audioSource.Stop();
    // }
}
