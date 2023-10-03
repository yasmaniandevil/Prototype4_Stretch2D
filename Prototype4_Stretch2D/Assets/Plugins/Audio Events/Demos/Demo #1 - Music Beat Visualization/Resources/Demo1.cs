namespace Dimension19.AudioEvents.DemoScenes
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Demo1 : MonoBehaviour
    {
        public AudioEventsComponent audioEventsComponent;

        #region Buttons

        [Header("Buttons")]
        public Button playButton;
        public Button stopButton;

        private void Awake()
        {
            playButton.onClick.AddListener(PlayButton);
            stopButton.onClick.AddListener(StopButton);
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

        [ContextMenu("Play audio from 5 seconds until 10 seconds")]
        public void PlayFrom5Till10Seconds()
        {
            audioEventsComponent.PlayFromTimeUntilTime(5f, 10f);
        }

        #endregion
    }
}