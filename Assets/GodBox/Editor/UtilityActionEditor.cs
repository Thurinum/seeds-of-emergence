using System;
using System.Linq;
using System.Collections.Generic;
using GodBox.UtilityAI;
using GodBox.UtilityAI.Considerations;
using UnityEditor;
using UnityEngine;

namespace GodBox.Editor
{
    [CustomEditor(typeof(UtilityAction), true)]
    public class UtilityActionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            var action = (UtilityAction)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Considerations", EditorStyles.boldLabel);

            // Manual list drawing to support adding polymorphic types conveniently
            for (int i = 0; i < action.Considerations.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                var consideration = action.Considerations[i];
                var title = consideration != null ? consideration.GetType().Name : "Null";
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    action.Considerations.RemoveAt(i);
                    // Must mark dirty to save changes
                    EditorUtility.SetDirty(action); 
                    i--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                // Draw properties of the consideration
                // Note: SerializeReference drawing is tricky in simple IMGUI.
                // We'll use PropertyDrawer logic via SerializeObject iterators for the list elements
                
                SerializedProperty listProp = serializedObject.FindProperty("Considerations");
                SerializedProperty elementProp = listProp.GetArrayElementAtIndex(i);
                
                // Iterate through children of this element
                SerializedProperty endProp = elementProp.GetEndProperty();
                SerializedProperty childProp = elementProp.Copy();
                if (childProp.NextVisible(true))
                {
                   while (!SerializedProperty.EqualContents(childProp, endProp))
                   {
                       EditorGUILayout.PropertyField(childProp, true);
                       if (!childProp.NextVisible(false)) break;
                   } 
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("Add Consideration"))
            {
                var menu = new GenericMenu();
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(Consideration).IsAssignableFrom(p) && !p.IsAbstract);

                foreach (var type in types)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () => 
                    {
                        var newCons = (Consideration)Activator.CreateInstance(type);
                        action.Considerations.Add(newCons);
                        EditorUtility.SetDirty(action);
                    });
                }
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
