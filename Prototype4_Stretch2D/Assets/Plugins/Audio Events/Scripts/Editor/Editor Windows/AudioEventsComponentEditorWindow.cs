namespace Dimension19.AudioEvents.EditorWindows
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using Dimension19.AudioEvents;

    public class AudioEventsComponentEditorWindow : ExtendedEditorWindow
    {
        private Texture2D image;
        private static AudioEventsComponent audioEventsComponent;

        #region Top Section Variables

        //public float top_Section_Width = 400f;
        public float top_Section_Height = 400f;

        #region Properties Section Variables

        // Properties Section Variables
        public float properties_Section_Width = 300f;
        //public float properties_Section_Height = 395f;

        // Audio Clip Variables
        public static AudioClip audioClip = null;
        private static GUIContent audioClipContent = new GUIContent("Audio Clip", "The AudioClip associated with this object.");

        //public static float audioClipVolume = 1f;
        private static GUIContent audioClipVolumeContent = new GUIContent("Audio Clip Volume", "The AudioClip's Volume associated with this object.");

        #endregion

        #region Audio Clip Waveform Section

        // Audio Clip Waveform Section Top Buttons
        public float audioClipWaveform_Section_TopButtons_Width = 250f;
        public float audioClipWaveform_Section_TopButtons_Height = 25f;

        public static Texture playIcon = null;
        public static Texture pauseIcon = null;
        public static Texture unpauseIcon = null;
        public static Texture stopIcon = null;

        public static Texture audioEventsIcon = null;

        public static Texture infoIcon = null;

        // Audio Clip Waveform Section Variables
        public float audioClipWaveform_Section_Width = 1000f;
        public float audioClipWaveform_Section_Height = 200f;

        public float audioWaveform_Width = 0f;

        private bool isPaused = false;
        private bool isPlaying = false;

        private int audioClipWaveform_ZoomMagnifcationMultiplier_Min = 1;
        private int audioClipWaveform_ZoomMagnifcationMultiplier_Max = 20;

        private int audioClipWaveform_ZoomMagnificationMultiplier = 1;

        // Audio Clip Waveform Variables
        private AudioWaveform audioWaveform = null;
        private Vector2 audioWaveform_ScrollPos;
        //private Texture audioWaveformTexture;
        private Texture[] audioWaveformTextures;
        private double audioWaveform_CursorHoverOverAtTime = 0f;
        private Texture2D audioPlayhead = null;
        private Texture2D audioPlayheadTopper = null;
        private float playheadPosition = 0f;

        private Texture2D audioPreviewPlayhead = null; // The Playhead used for when the Audio is actually being played

        private bool clickedInsideWaveform = false;

        #endregion

        #endregion

        private Texture2D audioEventsMarkerLine = null;
        //private Texture2D audioEventsButtonImage = null;

        private Texture2D audioWaveformTexture2D = null;

        //private float audioWaveform_InitialOffset = 5f;
        //private static GUIContent audioWaveform_InitialOffset_Content = new GUIContent("Visual Waveform Offset", "This controls the offset of the Audio Clip Waveform.");

        private Color audioWaveformBackgroundColor = new Color(0f, 28f, 31f);
        private static GUIContent audioWaveformBackgroundColorContent = new GUIContent("Background Color", "The background color of the Audio Waveform.");

        private Color audioWaveformForegroundColor = new Color(255f, 193f, 8f);
        private static GUIContent audioWaveformForegroundColorContent = new GUIContent("Foreground Color", "The foreground color of the Audio Waveform.");

        //private Color audioWaveformMarkerColor = Color.green;
        //private static GUIContent audioWaveformMarkerColorContent = new GUIContent("Marker Color", "The color of the Audio Event marker.");

        #region Bottom Section Variables

        #region Events Section

        private Vector2 events_ScrollPos;

        #endregion

        #region Event Details Section

        private Vector2 eventDetails_ScrollPos;
        [HideInInspector] public static int latestAudioEventArrayElementFocused = -1;

        #endregion

        #endregion

        /// <summary>
        /// Shows the Editor Window
        /// </summary>
        // Using ShowWindow() instead of Init() allows multiple instances to be created
        public static void ShowWindow(AudioEventsComponent _audioEventsComponent)
        {
            AudioEventsComponentEditorWindow window = GetWindow<AudioEventsComponentEditorWindow>(); // GetWindow is a method inherited from the EditorWindow class
            //window.Show();
            window.titleContent = new GUIContent("Audio Events Editor"); // The Title of the Window
            window.minSize = new Vector2(900f, 600f); // The Minimum Window Size

            audioEventsComponent = _audioEventsComponent; // Assigning and keeping the AudioEventsComponent object
            window.serializedObject = new SerializedObject(audioEventsComponent); // Serializing the Object

            audioClip = audioEventsComponent.audioSource.clip; // Assign the Audio Clip
            if (audioClip != null)
            {
                window.SendEvent(EditorGUIUtility.CommandEvent("DrawAudioWaveform")); // Draw the Audio Waveform on Startup
                window.SendEvent(EditorGUIUtility.CommandEvent("DrawArrayButtons"));
            }
            else
            {

            }

            if (audioEventsComponent.audioEvents.Length > 0)
            {
                latestAudioEventArrayElementFocused = 0;
            }
            else
            {
                latestAudioEventArrayElementFocused = -1;
            }
        }

        private void OnEnable()
        {
            //audioEventsButtonImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Audio Events/Editor/Gizmos/audioEventFlag.png", typeof(Texture2D));

            // Create a new Audio Playhead empty texture to be used
            audioPlayhead = new Texture2D(1, 1);
            audioPlayheadTopper = EditorGUIUtility.Load("Audio Events/Textures/AE_PlayheadTopper.png") as Texture2D;
            audioPreviewPlayhead = new Texture2D(1, 1);

            audioEventsMarkerLine = new Texture2D(1, 1);
            audioEventsMarkerLine.SetPixel(0, 0, AudioWaveform.marker);
            audioEventsMarkerLine.Apply();

            audioWaveformTexture2D = new Texture2D(1, 1);
            audioWaveformTexture2D.SetPixel(0, 0, audioWaveformBackgroundColor);
            audioWaveformTexture2D.wrapMode = TextureWrapMode.Repeat;
            audioWaveformTexture2D.Apply();

            playIcon = EditorGUIUtility.Load("Audio Events/Textures/AE_Play.png") as Texture;
            pauseIcon = EditorGUIUtility.Load("Audio Events/Textures/AE_Pause.png") as Texture;
            unpauseIcon = EditorGUIUtility.Load("Audio Events/Textures/AE_Play.png") as Texture;
            stopIcon = EditorGUIUtility.Load("Audio Events/Textures/AE_Stop.png") as Texture;

            audioEventsIcon = EditorGUIUtility.Load("Audio Events/Textures/audio-events-icon.png") as Texture;

            infoIcon = EditorGUIUtility.Load("Audio Events/Textures/AE_Info.png") as Texture;
        }

        /// <summary>
        /// On Window Destroy
        /// </summary>
        private void OnDestroy()
        {
            if (audioEventsComponent != null)
            {
                audioEventsComponent.ResetTimeValues();

                if (audioEventsComponent.audioSource != null)
                {
                    audioEventsComponent.audioSource.Stop(); // Stop any sound the Audio Source may be playing
                }
            }
        }

        /// <summary>
        /// On Validate
        /// </summary>
        private void OnValidate()
        {
            Repaint();
        }

        /// <summary>
        /// On GUI
        /// </summary>
        private void OnGUI()
        {
            if (serializedObject != null)
            {
                serializedObject.Update(); // Update the properties from the serialized object
            }
            else
            {
                EditorGUILayout.HelpBox("Please re-open this window.", MessageType.Info);
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                audioEventsComponent.ResetTimeValues();
            }

            #region Design Layout

            // The Initial Vertical Group
            // Occupied by: Entire Window
            GUILayout.BeginVertical();
            {
                // The Top Horizontal Group
                // Occupied by: Top Section
                GUILayout.BeginHorizontal("box", GUILayout.Height(top_Section_Height));
                {
                    ///* DEBUG IMAGE */ GUI.DrawTexture(new Rect(0f, 0f, 10000f, 10000f), image);

                    // The Left-Side Vertical Group
                    // Occupied by: Properties Section
                    GUILayout.BeginVertical("box", GUILayout.Width(properties_Section_Width) , GUILayout.Height(top_Section_Height));
                    {
                        ///* DEBUG IMAGE */ GUI.DrawTexture(new Rect(0f, 0f, 10000f, 10000f), image);

                        #region Properties Section

                        // Properties Title
                        GUILayout.Label("Audio Source Properties", EditorStyles.boldLabel);   // The Label of this group
                        DrawUILine(Color.grey, 1);
                        //EditorGUI.indentLevel++;    // Make an indentation on the row
                        {
                            //GUI.enabled = false;

                            // Audio Clip Properties
                            audioClip = (AudioClip)EditorGUILayout.ObjectField(audioClipContent, audioClip, typeof(AudioClip), true); // The Audio Clip property
                            // Change the AudioClip in the attached AudioSource component
                            audioEventsComponent.audioSource.clip = audioClip;

                            if (audioClip != null)
                            {
                                Rect audioWaveform_Buttons_Area = new Rect(11f, 64f, 292f, 15f);
                                audioEventsComponent.audioSource.volume = EditorGUI.Slider(audioWaveform_Buttons_Area, "Audio Clip Volume", audioEventsComponent.audioSource.volume, 0f, 1f);
                            }
                            else
                            {
                                GUI.enabled = false;
                                audioEventsComponent.audioSource.volume = EditorGUILayout.FloatField(audioClipVolumeContent, audioEventsComponent.audioSource.volume);
                                GUI.enabled = true;
                            }

                            //GUI.enabled = true;
                        }

                        #endregion

                        GUILayout.Space(25f);

                        #region Audio Waveform Settings Section

                        // Properties Title
                        GUILayout.Label("Audio Waveform Settings", EditorStyles.boldLabel);   // The Label of this group
                        DrawUILine(Color.grey, 1);

                        EditorGUI.BeginChangeCheck();

                        // 
                        //audioWaveform_InitialOffset = EditorGUILayout.FloatField(audioWaveform_InitialOffset_Content, audioWaveform_InitialOffset);

                        // Background Color
                        if (EditorPrefs.HasKey("audioWaveformBackgroundColor"))
                        {
                            string[] storedAudioWaveformBackgroundColor = EditorPrefs.GetString("audioWaveformBackgroundColor").Split('|');
                            Color savedAudioWaveformBackgroundColor = new Color(float.Parse(storedAudioWaveformBackgroundColor[0]), float.Parse(storedAudioWaveformBackgroundColor[1]), float.Parse(storedAudioWaveformBackgroundColor[2]), float.Parse(storedAudioWaveformBackgroundColor[3]));
                            audioWaveformBackgroundColor = savedAudioWaveformBackgroundColor;
                            audioWaveformBackgroundColor = EditorGUILayout.ColorField(audioWaveformBackgroundColorContent, audioWaveformBackgroundColor);
                        }
                        else
                        {
                            audioWaveformBackgroundColor = EditorGUILayout.ColorField(audioWaveformBackgroundColorContent, audioWaveformBackgroundColor);
                        }

                        // Foreground Color
                        if (EditorPrefs.HasKey("audioWaveformBackgroundColor"))
                        {
                            string[] storedAudioWaveformForegroundColor = EditorPrefs.GetString("audioWaveformForegroundColor").Split('|');
                            Color savedAudioWaveformForegroundColor = new Color(float.Parse(storedAudioWaveformForegroundColor[0]), float.Parse(storedAudioWaveformForegroundColor[1]), float.Parse(storedAudioWaveformForegroundColor[2]), float.Parse(storedAudioWaveformForegroundColor[3]));
                            audioWaveformForegroundColor = savedAudioWaveformForegroundColor;
                            audioWaveformForegroundColor = EditorGUILayout.ColorField(audioWaveformForegroundColorContent, audioWaveformForegroundColor);
                        }
                        else
                        {
                            audioWaveformForegroundColor = EditorGUILayout.ColorField(audioWaveformForegroundColorContent, audioWaveformForegroundColor);
                        }

                        // Marker Color
                        //audioWaveformMarkerColor = EditorGUILayout.ColorField(audioWaveformMarkerColorContent, AudioWaveform.marker);
                        //AudioWaveform.marker = audioWaveformMarkerColor;

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorPrefs.SetString("audioWaveformBackgroundColor", audioWaveformBackgroundColor.r + "|" + audioWaveformBackgroundColor.g + "|" + audioWaveformBackgroundColor.b + "|" + audioWaveformBackgroundColor.a);
                            EditorPrefs.SetString("audioWaveformForegroundColor", audioWaveformForegroundColor.r + "|" + audioWaveformForegroundColor.g + "|" + audioWaveformForegroundColor.b + "|" + audioWaveformForegroundColor.a);

                            SendEvent(EditorGUIUtility.CommandEvent("DrawAudioWaveform")); // Draw the Audio Waveform on Startup
                            GUIUtility.ExitGUI();
                        }

                        //EditorGUILayout.HelpBox("May have to restart the Editor Window to see the changes.", MessageType.Info);

                        #endregion
                    }
                    GUILayout.EndVertical();

                    // The Right-Side Vertical Group
                    // Occupied by: Audio Clip Waveform Section
                    GUILayout.BeginVertical("box", GUILayout.Height(top_Section_Height));
                    {
                        ///* DEBUG IMAGE */ GUI.DrawTexture(new Rect(0f, 0f, 10000f, 10000f), image);

                        #region Audio Waveform Buttons Top

                        // Audio Clip Waveform Top Buttons
                        GUILayout.BeginHorizontal("box", GUILayout.Height(audioClipWaveform_Section_TopButtons_Height));

                        // Audio Events logo
                        if (audioEventsIcon != null)
                        {
                            GUILayout.Label(audioEventsIcon);
                        }

                        GUILayout.FlexibleSpace();
                        //GUI.BeginGroup(new Rect(GUI.skin.box.margin.left, GUI.skin.box.margin.top, audioClipWaveform_Section_TopButtons_Width, audioClipWaveform_Section_TopButtons_Height));
                        {
                            ///* DEBUG IMAGE */ GUI.DrawTexture(new Rect(0f, 0f, 10000f, 10000f), image);

                            if (audioClip != null)
                            {
                                // Audio is playing
                                if (isPlaying)
                                {
                                    // Audio is paused
                                    if (isPaused)
                                    {
                                        if (unpauseIcon != null)
                                        {
                                            if (GUILayout.Button(unpauseIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "UnPause");
                                                audioEventsComponent.audioSource.UnPause();
                                                isPaused = false;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("UnPause"))
                                            {
                                                //Debug.Log("Clicked Button: " + "UnPause");
                                                audioEventsComponent.audioSource.UnPause();
                                                isPaused = false;
                                            }
                                        }

                                        // Stop Button
                                        if (stopIcon != null)
                                        {
                                            if (GUILayout.Button(stopIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Stop"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                    }
                                    // Audio is NOT paused
                                    else
                                    {
                                        // Pause Button
                                        if (pauseIcon != null)
                                        {
                                            if (GUILayout.Button(pauseIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Pause");
                                                audioEventsComponent.audioSource.Pause();
                                                isPaused = true;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Pause"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Pause");
                                                audioEventsComponent.audioSource.Pause();
                                                isPaused = true;
                                            }
                                        }

                                        // Stop Button
                                        if (stopIcon != null)
                                        {
                                            if (GUILayout.Button(stopIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Stop"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                    }
                                }
                                // Audio is NOT playing
                                else
                                {
                                    // Audio is paused
                                    if (isPaused)
                                    {
                                        // Stop Button
                                        if (stopIcon != null)
                                        {
                                            if (GUILayout.Button(stopIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Stop"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                    }
                                    // Audio is NOT paused
                                    else
                                    {
                                        // Play Button
                                        if (playIcon != null)
                                        {
                                            if (GUILayout.Button(playIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Play");
                                                audioEventsComponent.audioSource.time = (float)audioWaveform_CursorHoverOverAtTime; // Place the time the Audio is played to the selected area
                                                audioEventsComponent.audioSource.Play();
                                                isPaused = false;
                                                isPlaying = true;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Play"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Play");
                                                audioEventsComponent.audioSource.time = (float)audioWaveform_CursorHoverOverAtTime; // Place the time the Audio is played to the selected area
                                                audioEventsComponent.audioSource.Play();
                                                isPaused = false;
                                                isPlaying = true;
                                            }
                                        }

                                        // Stop Button
                                        if (stopIcon != null)
                                        {
                                            if (GUILayout.Button(stopIcon))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button("Stop"))
                                            {
                                                //Debug.Log("Clicked Button: " + "Stop");
                                                audioEventsComponent.audioSource.Stop();
                                                isPaused = false;
                                                isPlaying = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GUILayout.FlexibleSpace();

                        if (infoIcon != null)
                        {
                            if (GUILayout.Button(new GUIContent(infoIcon, "Opens Audio Events Online Documentation.")))
                            {
                                //Debug.Log("Clicked Button: " + "Info");
                                Application.OpenURL("https://docs.google.com/document/d/1fyuXbbsTqz8c9NHJpsQkSkhCx2F_J2ANCRfMcRAr818/");
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(new GUIContent("Info", "Opens Audio Events Online Documentation.")))
                            {
                                //Debug.Log("Clicked Button: " + "Info");
                                Application.OpenURL("https://docs.google.com/document/d/1fyuXbbsTqz8c9NHJpsQkSkhCx2F_J2ANCRfMcRAr818/");
                            }
                        }

                        GUILayout.EndHorizontal();

                        #endregion

                        #region Audio Waveform

                        Color bgColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.gray; //audioWaveformBackgroundColor;

                        Rect audioWaveform_Area = new Rect(properties_Section_Width + (GUI.skin.box.margin.horizontal * 2),                                                                                     // x
                                                           audioClipWaveform_Section_TopButtons_Height + (GUI.skin.box.margin.top * 4),                                                                         // y
                                                           position.width - properties_Section_Width - (GUI.skin.box.margin.right * 7),                                                                         // Width
                                                           top_Section_Height - audioClipWaveform_Section_TopButtons_Height - (GUI.skin.box.margin.bottom * 4) - audioClipWaveform_Section_TopButtons_Height);  // Height
                        EditorGUIUtility.AddCursorRect(new Rect(audioWaveform_Area.x, audioWaveform_Area.y + 3f, audioWaveform_Area.width, audioWaveform_Area.height - 20f), MouseCursor.Text); // Change the cursor when hovering over the Audio Waveform
                        GUILayout.BeginArea(audioWaveform_Area);
                        GUILayout.BeginHorizontal("box");
                        {
                            GUI.backgroundColor = bgColor;

                            // Horizontal Scroll View for Audio Waveform
                            audioWaveform_ScrollPos = EditorGUILayout.BeginScrollView(audioWaveform_ScrollPos);
                            {
                                if (audioClip != null)
                                {
                                    GUILayout.BeginHorizontal(GUILayout.Width(audioEventsComponent.audioSource.clip.length * audioClipWaveform_ZoomMagnificationMultiplier * 20));

                                    #region Draw Audio Waveform Event

                                    // On "DrawAudioWaveform" event called
                                    if (Event.current.commandName == "DrawAudioWaveform")
                                    {
                                        //Debug.Log("[AUDIO EVENTS/EDITOR WINDOW]: Drawing Waveform.");

                                        // Create a new AudioWaveform
                                        audioWaveform = new AudioWaveform(audioEventsComponent.audioSource);

                                        // If there is an AudioClip
                                        if (audioClip != null)
                                        {
                                            // If there is an Audio Waveform
                                            if (audioWaveform != null)
                                            {
                                                //Debug.Log("Drawing Waveform: " + audioWaveform);

                                                // Draw and store the Audio Waveform as a Texture to use
                                                //audioWaveformTexture = audioWaveform.GetWaveformAsTexture((int)audioClip.length * audioClipWaveform_ZoomMagnifcationMultiplier_Max * 20, (int)audioWaveform_Area.height - 20, 0.5f);
                                                audioWaveformTextures = audioWaveform.GetWaveformAsTextures(audioWaveformBackgroundColor, audioWaveformForegroundColor, ((int)audioClip.length + 1) * audioClipWaveform_ZoomMagnifcationMultiplier_Max * 20, (int)audioWaveform_Area.height - 20, 0.7f);
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Draw Waveform

                                    // Draw Audio Waveform
                                    float xPos = 0f * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max;
                                    for (int i = 0; i < audioWaveformTextures.Length; i++)
                                    {
                                        GUI.DrawTexture(new Rect(xPos, 0f, audioClip.length / audioWaveformTextures.Length * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max, audioWaveform_Area.height - 20f), audioWaveformTextures[i]); // Draw the current texture
                                        xPos += audioClip.length / audioWaveformTextures.Length * audioClipWaveform_ZoomMagnificationMultiplier * 20; // Calculation of where to draw the next Audio Waveform texture
                                    }
                                    audioWaveform_Width = xPos;

                                    // Draw Audio Events to be placed on top of the Waveform
                                    DrawAudioEventsOnWaveform(new Rect(playheadPosition * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier, 0f, 1f, audioWaveform_Area.height - 20f));

                                    // Draw Audio Playhead on top of the Waveform
                                    GUI.DrawTexture(new Rect(Mathf.Clamp(playheadPosition * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier, 0f, xPos), 0f, 1f, audioWaveform_Area.height - 20f), audioPlayhead);
                                    GUI.DrawTexture(new Rect(Mathf.Clamp(playheadPosition * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier - 10f / 2f, -10f / 2f, xPos - 10f / 2f), 0f, 10f, 17f), audioPlayheadTopper);

                                    #endregion

                                    // Mouse Position
                                    Event e = Event.current;
                                    GUILayout.Label(" "); // NEED THIS HERE FOR THE SCROLL VIEW TO WORK

                                    // FIX: Have to do this to fix the hovering over Audio Waveform
                                    Vector2 pos = e.mousePosition;
                                    pos.x += properties_Section_Width + (GUI.skin.box.margin.horizontal * 2) - audioWaveform_ScrollPos.x;
                                    pos.y += audioClipWaveform_Section_TopButtons_Height + (GUI.skin.box.margin.vertical * 2) - audioWaveform_ScrollPos.y;

                                    // Audio Waveform area NOT including the scroll bar
                                    if (new Rect(audioWaveform_Area.x, audioWaveform_Area.y, audioWaveform_Area.width, audioWaveform_Area.height - 20f).Contains(pos))
                                    {
                                        //audioWaveform_CursorHoverOverAtTime = audioEventsComponent.audioSource.clip.length * audioClipWaveform_ZoomMagnifcationMultiplier * 20;

                                        if (e.type == EventType.MouseDown && e.button == 0 || e.type == EventType.MouseDrag && e.button == 0)
                                        {
                                            clickedInsideWaveform = true;

                                            audioWaveform_CursorHoverOverAtTime = Mathf.Clamp((e.mousePosition.x / audioClipWaveform_ZoomMagnificationMultiplier / audioClipWaveform_ZoomMagnifcationMultiplier_Max), 0.0f, audioClip.length); // Update the time in the Audio Clip that the Cursor is currently hovered over
                                            playheadPosition = e.mousePosition.x / audioClipWaveform_ZoomMagnifcationMultiplier_Min / audioClipWaveform_ZoomMagnificationMultiplier; // Adjust the Playhead depending on how zoomed in you are

                                            audioEventsComponent.startFromTime = audioWaveform_CursorHoverOverAtTime; // Skip past events
                                        }
                                    }

                                    if (e.type == EventType.MouseUp && e.button == 0 && clickedInsideWaveform)
                                    {
                                        clickedInsideWaveform = false;
                                    }

                                    // Audio Waveform area INCLUDING the scroll bar
                                    if (audioWaveform_Area.Contains(pos))
                                    {
                                        // Right Click
                                        if (e.type == EventType.MouseUp && e.button == 1)
                                        {
                                            GenericMenu contextMenu = new GenericMenu(); // Create the Context Menu
                                            contextMenu.AddItem(new GUIContent("Add Event at " + audioWaveform_CursorHoverOverAtTime + " Seconds..."), false, () =>
                                            {
                                                Array.Resize(ref audioEventsComponent.audioEvents, audioEventsComponent.audioEvents.Length + 1);

                                                int arrayElement = audioEventsComponent.audioEvents.Length - 1; // Convert this to the element value
                                                audioEventsComponent.audioEvents[arrayElement] = new AudioEvent(); // Create the new Audio Event element
                                                audioEventsComponent.audioEvents[arrayElement].eventTime = audioWaveform_CursorHoverOverAtTime; // Assign the time to that of the Playhead

                                                latestAudioEventArrayElementFocused = arrayElement;
                                            });

                                            contextMenu.ShowAsContext(); // Show the Menu
                                        }

                                        // Scroll Wheel controls
                                        if (e.type == EventType.ScrollWheel)
                                        {
                                            // Scroll Up
                                            if (e.delta.y < 0f)
                                            {
                                                audioClipWaveform_ZoomMagnificationMultiplier = Mathf.Clamp(audioClipWaveform_ZoomMagnificationMultiplier + 1, audioClipWaveform_ZoomMagnifcationMultiplier_Min, audioClipWaveform_ZoomMagnifcationMultiplier_Max);
                                            }

                                            // Scroll Down
                                            if (e.delta.y > 0f)
                                            {
                                                audioClipWaveform_ZoomMagnificationMultiplier = Mathf.Clamp(audioClipWaveform_ZoomMagnificationMultiplier - 1, audioClipWaveform_ZoomMagnifcationMultiplier_Min, audioClipWaveform_ZoomMagnifcationMultiplier_Max);
                                            }

                                            audioWaveform_ScrollPos.x = playheadPosition + 0.1f * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier;

                                            Repaint(); // Repaint the Window if scrolling happened
                                        }
                                    }
                                    // Rect DOESNT contain Mouse Position
                                    else
                                    {
                                        // Do min and max audio waveform playhead stuff here
                                    }

                                    // Draw Preview Audio Playhead for the Audio Clip being currently played
                                    if (isPlaying)
                                    {
                                        GUI.DrawTexture(new Rect(audioEventsComponent.audioSource.time * audioClipWaveform_ZoomMagnifcationMultiplier_Max * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier, 0f, 1f, audioWaveform_Area.height - 20f), audioPreviewPlayhead);
                                    }

                                    GUILayout.EndHorizontal();
                                }
                                else
                                {
                                    GUILayout.BeginHorizontal(GUILayout.Width(position.width - properties_Section_Width - (GUI.skin.box.margin.left * 11)));
                                    GUILayout.Label("Audio Clip not available."); // Text to display when there is no Audio
                                    GUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndArea();

                        #endregion

                        #region Audio Waveform Info Bottom

                        // Audio Clip Waveform Bottom Buttons
                        Rect audioWaveform_Area_Bottom = new Rect(properties_Section_Width + (GUI.skin.box.margin.horizontal * 2),
                                                            audioClipWaveform_Section_TopButtons_Height + (GUI.skin.box.margin.top * 5) + audioWaveform_Area.height,
                                                            position.width - properties_Section_Width - (GUI.skin.box.margin.right * 7),
                                                            audioClipWaveform_Section_TopButtons_Height);
                        GUILayout.BeginArea(audioWaveform_Area_Bottom);
                        GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        {
                            if (audioClip != null)
                            {
                                GUILayout.Label("Audio Time: " + (float)audioWaveform_CursorHoverOverAtTime + " / " + audioClip.length);
                                Repaint();

                                // Zoom Slider
                                Rect audioWaveform_Buttons_Area = new Rect(position.width - 300f - properties_Section_Width - 32f, 3f, 300f, 15f);
                                audioClipWaveform_ZoomMagnificationMultiplier = EditorGUI.IntSlider(audioWaveform_Buttons_Area, "Zoom (Multiplier)", Mathf.RoundToInt(audioClipWaveform_ZoomMagnificationMultiplier), audioClipWaveform_ZoomMagnifcationMultiplier_Min, audioClipWaveform_ZoomMagnifcationMultiplier_Max);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndArea();

                        #endregion
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                // The Bottom Horizontal Group
                // Occupied by: Layers Section
                GUILayout.BeginHorizontal("box");
                {
                    // The Left-Side Vertical Group
                    // Occupied by: Properties Section
                    GUILayout.BeginVertical("box", GUILayout.Width(properties_Section_Width));
                    {
                        ///* DEBUG IMAGE */ GUI.DrawTexture(new Rect(0f, 0f, 10000f, 10000f), image);

                        #region Events Section

                        // Events Title
                        GUILayout.Label("Events", EditorStyles.boldLabel);   // The Label of this group
                        DrawUILine(Color.grey, 1);
                        //EditorGUI.indentLevel++; // Make an indentation on the row
                        {
                            events_ScrollPos = EditorGUILayout.BeginScrollView(events_ScrollPos);
                            {
                                if (audioClip != null)
                                {
                                    // Has to be updated every frame
                                    DrawArrayButtons(serializedObject.FindProperty("audioEvents"), "Audio Event", latestAudioEventArrayElementFocused); // Draws the Array buttons

                                    if (Event.current.commandName == "DrawArrayButtons")
                                    {

                                    }
                                }
                            }
                            EditorGUILayout.EndScrollView();

                            #region Add/Remove Array Elements

                            // The Horizontal Group for the Add and Remove array elements
                            GUILayout.BeginHorizontal();
                            {
                                if (audioClip != null)
                                {
                                    // Remove Button
                                    if (audioEventsComponent.audioEvents.Length > 0)
                                    {
                                        GUI.backgroundColor = Color.red;

                                        // Remove an element from the end of the Audio Events array
                                        if (GUILayout.Button(new GUIContent("Remove Last", "Removes the last Audio Event in the list.")))
                                        {
                                            Array.Resize(ref audioEventsComponent.audioEvents, audioEventsComponent.audioEvents.Length - 1);

                                            if (audioEventsComponent.audioEvents.Length <= 0)
                                            {
                                                latestAudioEventArrayElementFocused = -1;
                                                selectedProperty = null;
                                            }
                                            else if (latestAudioEventArrayElementFocused > audioEventsComponent.audioEvents.Length - 1)
                                            {
                                                latestAudioEventArrayElementFocused -= 1;
                                                selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(latestAudioEventArrayElementFocused);
                                            }

                                            DrawSelectedArrayElementPanel();
                                            GUI.FocusControl(null);
                                        }

                                        // Reset the background color to the default
                                        GUI.backgroundColor = Color.white;
                                    }
                                    else
                                    {
                                        GUI.enabled = false;

                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button(new GUIContent("Remove Last", "Removes the last Audio Event in the list.")))
                                        {
                                            // DO NOTHING SINCE BUTTON IS DEACTIVATED
                                        }

                                        GUI.enabled = true;

                                        // Reset the background color to the default
                                        GUI.backgroundColor = Color.white;
                                    }

                                    // Add Button
                                    GUI.backgroundColor = Color.green;
                                    // Add an extra element to the Audio Events array
                                    if (GUILayout.Button(new GUIContent("Add at Time", "Adds an Audio Event at the Playhead's current time.")))
                                    {
                                        Array.Resize(ref audioEventsComponent.audioEvents, audioEventsComponent.audioEvents.Length + 1);

                                        int arrayElement = audioEventsComponent.audioEvents.Length - 1; // Convert this to the element value

                                        // WE DO NOT COPY OVER THE UNITYEVENT REFERENCES BECAUSE ITS MORE COMPLEX THEN FIRST THOUGHT

                                        audioEventsComponent.audioEvents[arrayElement] = new AudioEvent(); // Create the new Audio Event element

                                        if (isPlaying)
                                        {
                                            audioEventsComponent.audioEvents[arrayElement].eventTime = audioEventsComponent.audioSource.time; // Assign the time to that of the Playhead
                                        }
                                        else
                                        {
                                            audioEventsComponent.audioEvents[arrayElement].eventTime = audioWaveform_CursorHoverOverAtTime; // Assign the time to that of the Playhead
                                        }

                                        latestAudioEventArrayElementFocused = arrayElement;
                                        serializedObject.Update();
                                        selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(latestAudioEventArrayElementFocused);

                                        GUI.FocusControl(null);
                                    }

                                    // + Add Button
                                    GUI.backgroundColor = Color.green;
                                    // Add an extra element to the Audio Events array
                                    if (GUILayout.Button(new GUIContent("+", "Adds an Audio Event."), GUILayout.Width(30f)))
                                    {
                                        Array.Resize(ref audioEventsComponent.audioEvents, audioEventsComponent.audioEvents.Length + 1);

                                        latestAudioEventArrayElementFocused = audioEventsComponent.audioEvents.Length - 1;
                                        serializedObject.Update();
                                        selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(latestAudioEventArrayElementFocused);

                                        GUI.FocusControl(null);
                                    }
                                }

                                // Reset the background color to the default
                                GUI.backgroundColor = Color.white;
                            }
                            GUILayout.EndHorizontal();

                            #endregion
                        }

                        #endregion
                    }
                    GUILayout.EndVertical();

                    // The Right-Side Vertical Group
                    GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
                    {
                        #region Event Details Section

                        GUILayout.Label("Event Details", EditorStyles.boldLabel); // The Label of this group
                        DrawUILine(Color.grey, 1);
                        {
                            eventDetails_ScrollPos = EditorGUILayout.BeginScrollView(eventDetails_ScrollPos);
                            {
                                if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand)
                                {
                                    if (Event.current.commandName == "DrawSelectedArrayElementPanel")
                                    {
                                        selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(latestAudioEventArrayElementFocused);
                                        DrawSelectedArrayElementPanel();
                                    }
                                }

                                // Check to see if there is a property currently selected
                                if (selectedProperty != null)
                                {
                                    DrawSelectedArrayElementPanel(); // Draw the currently selected array element's information
                                }
                                // Select the first Audio Event if the array contains anything
                                else if (serializedObject.FindProperty("audioEvents").arraySize > 0)
                                {
                                    // DOES THIS FOR 1 FRAME TO SET SELECTEDPROPERTY
                                    selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(0);
                                    DrawSelectedArrayElementPanel();
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Select an item from the Events list for it's details to be shown here."); // Display this message if there is NOT a selected property currently
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }

                        #endregion
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            #endregion

            // Check if anything on the GUI has changed
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties(); // Apply all properties that were modified
            }
        }

        /// <summary>
        /// Draw the currently selected array element's information in a panel
        /// </summary>
        private void DrawSelectedArrayElementPanel()
        {
            if (audioClip != null)
            {
                currentProperty = selectedProperty;

                if (currentProperty != null && latestAudioEventArrayElementFocused > -1)
                {
                    DrawField("eventName", true, "Custom Event Name");
                    DrawField("eventMarkerColor", true, "Custom Event Marker Color", 200f);

                    DrawUILine(Color.grey, 1, 10, 40f);

                    DrawField("eventTime", true, "Audio Event Time");
                    GUILayout.Space(5f);
                    DrawField("onReached", true, "On Reached Event");

                    GUILayout.Space(15f);
                    DrawUILine(Color.grey, 1, 10);
                    GUILayout.Space(15f);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(new GUIContent("Remove Audio Event", "Removes this currently selected Audio Event.")))
                    {
                        RemoveAt(ref audioEventsComponent.audioEvents, latestAudioEventArrayElementFocused);

                        latestAudioEventArrayElementFocused -= 1;

                        if (latestAudioEventArrayElementFocused > -1)
                        {
                            currentProperty = selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(latestAudioEventArrayElementFocused);
                        }
                        else
                        {
                            latestAudioEventArrayElementFocused = -1;
                            currentProperty = selectedProperty = null;
                        }

                        GUI.FocusControl(null);
                    }

                    // Reset the background color to the default
                    GUI.backgroundColor = Color.white;
                }
                else
                {
                    EditorGUILayout.LabelField("Select an item from the Events list for it's details to be shown here."); // Display this message if there is NOT a selected property currently
                }
            }
        }

        /// <summary>
        /// Draw all Audio Events on top of the Audio Waveform
        /// </summary>
        private void DrawAudioEventsOnWaveform(Rect area)
        {
            int it = 0;

            //GUI.skin.button.normal = buttonDeactive;

            for (int i = 0; i < audioEventsComponent.audioEvents.Length; i++)
            {
                // Change the color of the Audio Event Marker Line
                audioEventsMarkerLine.SetPixel(0, 0, audioEventsComponent.audioEvents[i].eventMarkerColor);
                audioEventsMarkerLine.Apply();
                // Draw the placed Audio Event marker line
                GUI.DrawTexture(new Rect((float)audioEventsComponent.audioEvents[i].eventTime * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max, 0f, 1f, area.height), audioEventsMarkerLine);

                Vector2 audioEventMarkerSize = new Vector2(30f, 30f);
                Vector2 audioEventMarkerOffset = new Vector2(0f, -30f);

                // Specify the area of the Audio Event button marker
                Rect audioEventButtonArea = new Rect((float)audioEventsComponent.audioEvents[i].eventTime * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max + audioEventMarkerOffset.x,
                                                            audioClipWaveform_Section_TopButtons_Height + (GUI.skin.box.margin.top * 4) + audioEventMarkerOffset.y,
                                                            audioEventMarkerSize.x,
                                                            audioEventMarkerSize.y);

                // If the button for the Audio Event marker is greater than the area of the Audio Waveform
                if ((audioEventButtonArea.x + audioEventButtonArea.width) > (audioClip.length * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max))
                {
                    audioEventButtonArea = new Rect((float)audioEventsComponent.audioEvents[i].eventTime * audioClipWaveform_ZoomMagnifcationMultiplier_Min * audioClipWaveform_ZoomMagnificationMultiplier * audioClipWaveform_ZoomMagnifcationMultiplier_Max - audioEventMarkerSize.x + audioEventMarkerOffset.x,
                                                            audioClipWaveform_Section_TopButtons_Height + (GUI.skin.box.margin.top * 4) + audioEventMarkerOffset.y,
                                                            audioEventMarkerSize.x,
                                                            audioEventMarkerSize.y);
                }

                EditorGUIUtility.AddCursorRect(audioEventButtonArea, MouseCursor.Arrow); // Change the cursor when hovering
                // Audio Event Marker Button Area
                GUILayout.BeginArea(audioEventButtonArea);
                {
                    it++;

                    GUI.backgroundColor = audioEventsComponent.audioEvents[i].eventMarkerColor;
                    // Marker button functionality
                    if (GUILayout.Button(new GUIContent(it.ToString(), "[" + it.ToString() + "]" + " Audio Event Marker")))
                    {
                        selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(i);
                        latestAudioEventArrayElementFocused = i;

                        GUI.FocusControl(null);
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndArea();
            }
        }
    }
}