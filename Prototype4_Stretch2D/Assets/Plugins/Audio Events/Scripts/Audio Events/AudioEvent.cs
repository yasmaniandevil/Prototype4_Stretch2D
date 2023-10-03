namespace Dimension19.AudioEvents
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// This class stores information and is positioned at a time in Audio.
    /// </summary>
    [System.Serializable]
    public class AudioEvent
    {
        public AudioEvent()
        {
            eventMarkerColor = Color.green;
        }

        public AudioEvent(UnityEvent _OnReached)
        {
            onReached = _OnReached;
        }

        public AudioEvent(Color _EventMarkerColor, double _EventTime = 0.0, UnityEvent _OnReached = null)
        {
            eventMarkerColor = _EventMarkerColor;

            eventTime = _EventTime;
            onReached = _OnReached;
        }

        // Custom identifiers
        public string eventName = ""; // The overridden custom Event name
        public Color eventMarkerColor = Color.green;

        // Constant identifiers
        public double eventTime = 0.0f; // The time in which the Event is invoked
        public bool hasBeenReached = false; // Has this Event been reached yet?
        public UnityEvent onReached = null; // The UnityEvent associated with this Event
    }
}