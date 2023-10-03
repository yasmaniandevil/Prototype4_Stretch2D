namespace Dimension19.AudioEvents.EditorWindows
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEditorInternal;

    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;

        //private string selectedPropertyPath;
        protected SerializedProperty selectedProperty;

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        /// <summary>
        /// Draws the SerializedProperty's Array elements as Buttons
        /// </summary>
        protected void DrawArrayButtons(SerializedProperty prop, string buttonNameIdentifier, int latestAudioEventArrayElementFocused)
        {
            //Debug.Log("DRAWING ARRAY BUTTONS");

            Texture2D buttonBackground = GUI.skin.button.normal.background;

            for (int i = 0; i < prop.arraySize; i++)
            {
                if (i == latestAudioEventArrayElementFocused)
                {
                    // IF ARRAY ELEMENT IS FOCUSED
                    GUI.skin.button.normal.background = GUI.skin.button.active.background;
                    GUI.backgroundColor = prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventMarkerColor").colorValue;

                    // If the Button does not have a custom name
                    if (!string.IsNullOrEmpty(prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventName").stringValue))
                    {
                        // Check to see if the user presses this button
                        if (GUILayout.Button("[" + (i + 1).ToString() + "] " + prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventName").stringValue))
                        {
                            GUI.FocusControl(null);
                        }
                    }
                    // If the Button has a custom name
                    else
                    {
                        // Check to see if the user presses this button
                        if (GUILayout.Button("[" + (i + 1).ToString() + "] " + buttonNameIdentifier))
                        {
                            GUI.FocusControl(null);
                        }
                    }
                }
                else
                {
                    // IF ARRAY ELEMENT ISNT FOCUSED
                    GUI.skin.button.normal.background = buttonBackground;
                    GUI.backgroundColor = prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventMarkerColor").colorValue;

                    // If the Button does not have a custom name
                    if (!string.IsNullOrEmpty(prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventName").stringValue))
                    {
                        // Check to see if the user presses this button
                        if (GUILayout.Button("[" + (i + 1).ToString() + "] " + prop.GetArrayElementAtIndex(i).FindPropertyRelative("eventName").stringValue))
                        {
                            selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(i);
                            AudioEventsComponentEditorWindow.latestAudioEventArrayElementFocused = i;

                            GUI.FocusControl(null);
                        }
                    }
                    // If the Button has a custom name
                    else
                    {
                        // Check to see if the user presses this button
                        if (GUILayout.Button("[" + (i + 1).ToString() + "] " + buttonNameIdentifier))
                        {
                            selectedProperty = serializedObject.FindProperty("audioEvents").GetArrayElementAtIndex(i);
                            AudioEventsComponentEditorWindow.latestAudioEventArrayElementFocused = i;

                            GUI.FocusControl(null);
                        }
                    }
                }

                GUI.skin.button.normal.background = buttonBackground;
            }

            GUI.skin.button.normal.background = buttonBackground;

            // Check to see if the array element button clicked has an appropriate path
            //if (!string.IsNullOrEmpty(selectedPropertyPath))
            //{
            //    selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
            //}
        }

        protected void DrawField(string propName, bool relative, string customPropName = "", float labelWidth = 0f)
        {
            // Check if a Custom Property Name has NOT been entered
            if (string.IsNullOrEmpty(customPropName))
            {
                customPropName = propName; // Set the Property Name as the one to use when drawing the Property
            }

            if (relative && currentProperty != null)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), new GUIContent(customPropName), true);
            }
            else if (serializedObject != null)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), new GUIContent(customPropName), true);
            }
        }

        protected void DrawUILine(Color color, int thickness = 2, int padding = 10, float width = 80f)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            //r.width += 6;
            r.width = width;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// Removes an Element from a referenced Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }
            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }
    }
}