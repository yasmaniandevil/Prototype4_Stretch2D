namespace Dimension19.AudioEvents
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Dimension19.AudioEvents.EditorWindows;
    using Dimension19.AudioEvents;

    [CustomEditor(typeof(AudioEventsComponent))]
    public class AudioEventsComponentEditor : Editor
    {
        // On the Inspector GUI
        public override void OnInspectorGUI()
        {
            // Get the instance of the focused AudioEvents object from the Inspector
            AudioEventsComponent myAudioEventsComponent = (AudioEventsComponent)target;

            GUI.backgroundColor = new Color(150f / 255f, 38f / 255f, 169f / 255f, 1f);
            GUI.skin.button.normal.textColor = new Color(255f / 255f, 193f / 255f, 8f / 255f, 1f);
            // Open the Audio Editor button
            if (GUILayout.Button("Open Audio Events Editor"))
            {
                #region Instance Window

                //AudioEventsComponentEditorWindow window = (AudioEventsComponentEditorWindow)CreateInstance(typeof(AudioEventsComponentEditorWindow));
                //window.ShowWindow(myAudioEventsComponent); // Call the Show Editor Window function and parse in the associated AudioEventsComponent object

                #endregion

                #region Static Window

                AudioEventsComponentEditorWindow.ShowWindow(myAudioEventsComponent); // Call the Show Editor Window function and parse in the associated AudioEventsComponent object

                #endregion
            }
            GUI.skin.button.normal.textColor = Color.black;
            GUI.backgroundColor = Color.white;

            // Draws all Default Inspector variables
            //DrawDefaultInspector();
        }
    }
}