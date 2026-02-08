using System.Linq;
using GodBox.UtilityAI;
using GodBox.UtilityAI.Considerations;
using UnityEditor;
using UnityEngine;

namespace GodBox.Editor
{
    public class UtilityAIDebugger : EditorWindow
    {
        private UtilityAIComponent _selectedAgent;
        private Vector2 _scrollPos;

        [MenuItem("GodBox/AI Debugger")]
        public static void ShowWindow()
        {
            GetWindow<UtilityAIDebugger>("AI Debugger");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Agent Selection", EditorStyles.boldLabel);
            
            // Find all agents
            var agents = FindObjectsByType<UtilityAIComponent>(FindObjectsSortMode.None);
            
            string[] names = agents.Select(a => a.name).ToArray();
            int selectedIndex = -1;
            
            if (_selectedAgent != null)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    if (agents[i] == _selectedAgent)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            int newIndex = EditorGUILayout.Popup("Select Agent", selectedIndex, names);
            if (newIndex >= 0 && newIndex < agents.Length)
            {
                _selectedAgent = agents[newIndex];
            }

            if (_selectedAgent == null)
            {
                EditorGUILayout.HelpBox("Select an agent to debug.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Debugging: {_selectedAgent.name}", EditorStyles.largeLabel);
            
            string activeActionName = "None";
            if (_selectedAgent.CurrentAction != null)
            {
                activeActionName = string.IsNullOrEmpty(_selectedAgent.CurrentAction.ActionName) 
                    ? _selectedAgent.CurrentAction.name 
                    : _selectedAgent.CurrentAction.ActionName;
            }
            EditorGUILayout.LabelField($"Active Action: {activeActionName}");

            EditorGUILayout.Space();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            // Show Needs if available
            var needs = _selectedAgent.GetComponent<BasicNeedsComponent>();
            if (needs != null)
            {
                EditorGUILayout.LabelField("Needs", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Food: {needs.FoodPercentage:P0}");
                EditorGUILayout.LabelField($"Health: {needs.HealthPercentage:P0}");
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Action Scores", EditorStyles.boldLabel);

            if (_selectedAgent.AvailableActions != null)
            {
                // We create a temporary list sorted by score for visualization
                var scoredActions = _selectedAgent.AvailableActions
                    .Select(a => new { Action = a, Score = a.Evaluate(_selectedAgent) })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                foreach (var item in scoredActions)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    if (_selectedAgent.CurrentAction == item.Action)
                    {
                        style.normal.background = MakeTex(1, 1, new Color(0.2f, 0.6f, 0.2f, 0.5f));
                    }

                    EditorGUILayout.BeginVertical(style);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item.Action.name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Total Score: {item.Score:F3}", GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();

                    // Show breakdown
                    if (item.Action.Considerations != null)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var cons in item.Action.Considerations)
                        {
                            if (cons == null) continue;
                            float rawInput = cons.GetInput(_selectedAgent);
                            float curveVal = cons.Evaluate(_selectedAgent); // Evaluate applies curve
                            EditorGUILayout.LabelField($"{cons.GetType().Name}: Input({rawInput:F2}) -> Curve({curveVal:F2})");
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
            }

            EditorGUILayout.EndScrollView();
            
            // Force repaint to keep live values updating
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
