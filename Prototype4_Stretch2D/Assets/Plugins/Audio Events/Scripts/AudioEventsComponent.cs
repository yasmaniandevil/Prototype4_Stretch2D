namespace Dimension19.AudioEvents
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Audio;

    /// <summary>
    /// This class is used to provide simple functionalities from the
    /// information provided in the Audio Events Editor Window.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioEventsComponent : MonoBehaviour
    {
        #region Variables

        #region States

        //// On Audio Start
        //public delegate void AudioStart();
        //public static event AudioStart OnAudioStart;
        //
        //// On Audio Playing
        //public delegate void AudioPlaying();
        //public static event AudioPlaying OnAudioPlaying;
        //
        //// On Audio Stop
        //public delegate void AudioStop();
        //public static event AudioStop OnAudioStop;

        // State Enum
        private enum AudioState
        {
            Stopped,
            Paused,
            Playing
        }

        private AudioState audioState = AudioState.Stopped; // State Variable
        private AudioState previousAudioState = AudioState.Stopped; // Previous State Variable

        private enum AudioStateTransition
        {
            Changing,
            StandBy
        }

        private AudioStateTransition audioStateTransition = AudioStateTransition.StandBy;

        private void ChangeState(AudioState _nextState)
        {
            // Check if the Next State is different than the Current State
            if (_nextState != audioState)
            {
                audioStateTransition = AudioStateTransition.Changing;
            }

            // Set the State variables
            previousAudioState = audioState; // Update previous state first
            audioState = _nextState; // Update current state
        }

        #endregion

        #region Audio

        // Audio Events Asset variable
        //public AudioEventsAsset audioEventsAsset = null;

        // Attached Audio Source variable
        [HideInInspector] public AudioSource audioSource = null;

        private float audioTimeLastFrame = 0.0f; // Used to check audio time progression

        #region Events

        [SerializeField] public AudioEvent[] audioEvents = new AudioEvent[0]; // Cache an empty AudioEvent array

        #endregion

        #endregion

        #endregion

        #region Basic Functionality

        [ContextMenu("Play Audio")]
        public void Play()
        {
            audioSource.Play();
        }

        [ContextMenu("Pause Audio")]
        public void Pause()
        {
            audioSource.Pause();
        }

        [ContextMenu("UnPause Audio")]
        public void UnPause()
        {
            audioSource.UnPause();
        }

        [ContextMenu("Stop Audio")]
        public void Stop()
        {
            audioSource.Stop();
        }

        #endregion

        #region Unity Functions

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Insepctor's context menu or when adding the component the first time.
        /// This function is only called in editor mode.
        /// </summary>
        private void Reset()
        {
            audioSource = GetAudioSourceComponent();
        }

        /// <summary>
        /// OnValidate is called when the script is loaded or a value is changed in the Inspector (called in the Editor only)
        /// </summary>
        private void OnValidate()
        {
            GetAudioSourceComponentsVariables();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            // Detect and Change the Audio State
            DetectAudioState();

            // Audio States Functionality
            AudioStateFunctionality();

            // This has to be at the end of the Update() function
            audioTimeLastFrame = audioSource.time;
            // This has to be at the end of the Update() function
            audioStateTransition = AudioStateTransition.StandBy;
        }

        #endregion

        #region Custom Functions

        #region Inspector Functions

        /// <summary>
        /// Gets and Sets the attached AudioSource's variables to the variables in this class
        /// </summary>
        public void GetAudioSourceComponentsVariables()
        {
            audioSource = GetAudioSourceComponent();
        }

        /// <summary>
        /// Get the AudioSource component attached to this object
        /// </summary>
        private AudioSource GetAudioSourceComponent()
        {
            // Try to get the AudioSource component on this object
            audioSource = GetComponent<AudioSource>();

            // Validation check to see if there was an AudioSource component
            // If there wasn't an AudioSource component added already:
            if (audioSource == null)
            {
                Debug.Log("[Audio Events]: Could not find an AudioSource component attached to this object. Adding AudioSource component now.");

                // Add an AudioSource component to this object
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            return audioSource;
        }

        #endregion

        #region State Functions

        /// <summary>
        /// Called from the Update() function
        /// Detects and changes the Audio States
        /// </summary>
        private void DetectAudioState()
        {
            // Check if the Audio Source is currently playing
            if (audioSource.isPlaying)
            {
                // Check if the time of the Audio is LESS THAN last frames
                // This helps with when the AudioSource.Play() is called when the Audio is already being played
                if (audioSource.time < audioTimeLastFrame)
                {
                    ChangeState(AudioState.Playing); // Change State to Playing just in-case it isn't already

                    ResetAllAudioEvents(); // Reset all the Audio Events

                    return;
                }

                ChangeState(AudioState.Playing); // Change State to Playing
            }
            else
            {
                // Check if the Audio Source is NOT at 0f (meaning it is MOST LIKELY paused)
                if (audioSource.time != 0f)
                {
                    ChangeState(AudioState.Paused); // Change State to Paused

                    return;
                }

                ChangeState(AudioState.Stopped); // Change State to Stopped
            }
        }

        /// <summary>
        /// Called from the Update() function
        /// Used to control any of the Audio State functionality
        /// </summary>
        private void AudioStateFunctionality()
        {
            // Track the Audio if it is playing
            if (audioState == AudioState.Playing)
            {
                TrackAudio();
            }
            // Reset all Audio Events once stopped
            else if (audioState == AudioState.Stopped && audioStateTransition == AudioStateTransition.Changing)
            {
                ResetAllAudioEvents();
            }
        }

        #endregion

        #region Audio Tracking

        public double startFromTime = 0.0; // The time in which the Audio has been started at
        public double endAtTime = 0.0; // The time in which the Audio has been set to end at

        /// <summary>
        /// Track Audio and invoke audio events
        /// </summary>
        private void TrackAudio()
        {
            // Stop Audio Source if the End time is reached
            if (endAtTime > 0.0 && audioSource.time >= endAtTime)
            {
                //Debug.Log("End at Time reached.");
                audioSource.Stop();
                return;
            }

            // Go through each AudioEvent attached in the AudioEventsAsset
            for (int i = 0; i < audioEvents.Length; i++)
            {
                // Skip past all events that are before the selected starting time
                if (audioEvents[i].eventTime < startFromTime)
                {
                    //Debug.Log("Skipped: " + audioEvents[i].eventTime + " from: " + startFromTime);

                    // Mark that this AudioEvent has been reached
                    audioEvents[i].hasBeenReached = true;
                }

                // Check if the AudioEvent has been reached yet
                // If the AudioEvent has NOT been reached yet
                if (audioEvents[i].hasBeenReached == false)
                {
                    // If the AudioSource's current time is GREATER THAN/EQUAL to the AudioEvent's set time
                    if (audioEvents[i].eventTime <= audioSource.time)
                    {
                        // Mark that this AudioEvent has been reached
                        audioEvents[i].hasBeenReached = true;

                        // Invoke Event
                        audioEvents[i].onReached.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Reset All of the Audio Events Asset's audio events
        /// </summary>
        private void ResetAllAudioEvents()
        {
            //Debug.Log("RESETTING ALL AUDIO EVENTS!");

            // Reset time values
            ResetTimeValues();

            // Go through each AudioEvent attached in the AudioEventsAsset
            for (int i = 0; i < audioEvents.Length; i++)
            {
                // Reset all temporary variables
                audioEvents[i].hasBeenReached = false;
            }
        }

        #endregion

        #region Public Functions

        public void ResetTimeValues()
        {
            //Debug.Log("Resetting time values");

            startFromTime = 0.0; // The time in which the Audio has been started at
            endAtTime = 0.0; // The time in which the Audio has been set to end at
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired time (in seconds) into the clip.
        /// </summary>
        /// <param name="_SecondsIn"></param>
        public void PlayFromTime(float _SecondsIn)
        {
            startFromTime = _SecondsIn;
            audioSource.time = _SecondsIn;
            audioSource.Play();
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired Audio Event's time (in seconds) into the clip.
        /// </summary>
        /// <param name="_EventName"></param>
        public void PlayFromEvent(string _EventName)
        {
            for (int i = 0; i < audioEvents.Length; i++)
            {
                if (audioEvents[i].eventName == _EventName)
                {
                    startFromTime = audioEvents[i].eventTime;
                    audioSource.time = (float)audioEvents[i].eventTime;
                    audioSource.Play();

                    return;
                }
            }
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired Audio Event's time (in seconds) into the clip.
        /// </summary>
        /// <param name="_EventNumber"></param>
        public void PlayFromEvent(int _EventNumber)
        {
            for (int i = 0; i < audioEvents.Length; ++i)
            {
                if (i == _EventNumber)
                {
                    startFromTime = audioEvents[i].eventTime;
                    audioSource.time = (float)audioEvents[i].eventTime;
                    audioSource.Play();

                    return;
                }
            }
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired Audio Event's time (in seconds) into the clip.
        /// </summary>
        /// <param name="_Event"></param>
        public void PlayFromEvent(AudioEvent _Event)
        {
            startFromTime = _Event.eventTime;
            audioSource.time = (float)_Event.eventTime;
            audioSource.Play();
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired time (in seconds) to a greater time (in seconds).
        /// </summary>
        /// <param name="_FromTime"></param>
        /// <param name="_ToTime"></param>
        public void PlayFromTimeUntilTime(float _FromTime = 0f, float _ToTime = 0f)
        {
            startFromTime = _FromTime;
            audioSource.time = (float)startFromTime;

            endAtTime = _ToTime;

            if (_ToTime < _FromTime)
            {
                Debug.LogWarning("[Audio Events]: _ToTime needs to be greater than _FromTime.");
                return;
            }

            else if (_ToTime == _FromTime)
            {
                Debug.Log("[Audio Events]: _ToTime needs to be greater than the _FromTime. Setting _ToTime to the AudioSource Component's audio clip's length (in seconds).");
                endAtTime = audioSource.clip.length;
            }

            audioSource.Play();
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired Audio Event's time (in seconds) to a greater Audio Event's time (in seconds).
        /// </summary>
        /// <param name="_StartEvent"></param>
        /// <param name="_EndEvent"></param>
        public void PlayFromEventUntilEvent(AudioEvent _StartEvent, AudioEvent _EndEvent)
        {
            startFromTime = _StartEvent.eventTime;
            audioSource.time = (float)startFromTime;

            endAtTime = _EndEvent.eventTime;

            if (_EndEvent.eventTime < _StartEvent.eventTime)
            {
                Debug.LogWarning("[Audio Events]: _EndEvent time needs to be greater than the _StartEvent time.");
                return;
            }

            else if (_EndEvent.eventTime == _StartEvent.eventTime)
            {
                Debug.Log("[Audio Events]: _EndEvent time needs to be greater than the _StartEvent time. Setting _EndEvent time to the AudioSource Component's audio clip's length (in seconds).");
                endAtTime = audioSource.clip.length;
            }

            audioSource.Play();
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired Audio Event's time (in seconds) to a greater time (in seconds).
        /// </summary>
        /// <param name="_FromEvent"></param>
        /// <param name="_ToTime"></param>
        public void PlayFromEventUntilTime(AudioEvent _FromEvent, float _ToTime)
        {
            startFromTime = _FromEvent.eventTime;
            audioSource.time = (float)startFromTime;

            endAtTime = _ToTime;

            if (_ToTime < _FromEvent.eventTime)
            {
                Debug.LogWarning("[Audio Events]: _ToTime needs to be greater than the _FromEvent time.");
                return;
            }

            else if (_ToTime == _FromEvent.eventTime)
            {
                Debug.Log("[Audio Events]: _ToTime needs to be greater than the _FromEvent time. Setting _ToTime to the AudioSource Component's audio clip's length (in seconds).");
                endAtTime = audioSource.clip.length;
            }

            audioSource.Play();
        }

        /// <summary>
        /// Plays the attached AudioSource component's Audio Clip from the desired time (in seconds) to a greater Audio Event's time (in seconds).
        /// </summary>
        /// <param name="_FromTime"></param>
        /// <param name="_ToEvent"></param>
        public void PlayFromTimeUntilEvent(float _FromTime, AudioEvent _ToEvent)
        {
            startFromTime = _FromTime;
            audioSource.time = (float)startFromTime;

            endAtTime = _ToEvent.eventTime;

            if (_ToEvent.eventTime < _FromTime)
            {
                Debug.LogWarning("[Audio Events]: _ToEvent time needs to be greater than the _FromTime.");
                return;
            }

            else if (_ToEvent.eventTime == _FromTime)
            {
                Debug.Log("[Audio Events]: _ToEvent time needs to be greater than the _FromTime. Setting _ToEvent time to the AudioSource Component's audio clip's length (in seconds).");
                endAtTime = audioSource.clip.length;
            }

            audioSource.Play();
        }

        /// <summary>
        /// Retrieves a desired Audio Event from this Audio Event Component's Audio Event array by the element's Event Number shown in the list (This is not to be confused with the array element's value).
        /// </summary>
        /// <param name="_EventNumber"></param>
        /// <returns></returns>
        public AudioEvent GetAudioEvent(int _EventNumber)
        {
            for (int i = 0; i < audioEvents.Length; ++i)
            {
                if (i == _EventNumber)
                {
                    return audioEvents[i];
                }
            }

            Debug.LogWarning("[Audio Events]: Could not find the desired Audio Event.");
            return new AudioEvent();
        }

        /// <summary>
        /// Retrieves a desired Audio Event from this Audio Event Component's Audio Event array by the element's Custom Event Name.
        /// </summary>
        /// <param name="_EventName"></param>
        /// <returns></returns>
        public AudioEvent GetAudioEvent(string _EventName)
        {
            for (int i = 0; i < audioEvents.Length; ++i)
            {
                if (audioEvents[i].eventName == _EventName)
                {
                    return audioEvents[i];
                }
            }

            Debug.LogWarning("[Audio Events]: Could not find the desired Audio Event.");
            return new AudioEvent();
        }

        #endregion

        #endregion
    }
}