namespace Dimension19.AudioEvents.DemoScenes
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Demo2 : MonoBehaviour
    {
        public AudioEventsComponent audioEventsComponent;

        #region Karaoke

        [Header("Karaoke")]
        public Text lyricText;

        public void ReadLyricLine(string _LyricLine)
        {
            lyricText.text = _LyricLine;
        }

        #endregion

        #region Buttons

        [Header("Buttons")]
        public Button playButton;
        public Button stopButton;

        private void Awake()
        {
            playButton.onClick.AddListener(PlayButton);
            stopButton.onClick.AddListener(StopButton);
        }

        private void Start()
        {
            SetVolumeLevel(audioEventsComponent.audioSource.volume);
        }

        private void PlayButton()
        {
            audioEventsComponent.audioSource.Play();
        }

        private void StopButton()
        {
            audioEventsComponent.audioSource.Stop();
        }

        public void SetVolumeLevel(float _sliderValue)
        {
            audioEventsComponent.audioSource.volume = _sliderValue; // Mathf.Log10(_sliderValue) * 20; // This comment lets the volume be controlled linearly
        }

        #endregion
    }
}