using System.Collections;
using UnityEngine;
using TMPro;

namespace GodBox.Conversation
{
    public class DialogueComponent : MonoBehaviour
    {
        public TMP_Text WorldText; // Supports both TextMeshPro and TextMeshProUGUI
        public float TypeSpeed = 0.05f;
        public float DisplayDuration = 3f;

        private Coroutine _typeRoutine;

        public void Say(string text)
        {
            // Debug.Log($"[DialogueComponent] Say called with: '{text}'");
            if (_typeRoutine != null) StopCoroutine(_typeRoutine);
            _typeRoutine = StartCoroutine(TypeWriter(text));
        }

        private IEnumerator TypeWriter(string text)
        {
            if (WorldText)
            {
                WorldText.text = "";
            }
            else
            {
                Debug.LogWarning($"[DialogueComponent] {name} tried to say '{text}' but WorldText is null!");
            }

            for (int i = 0; i <= text.Length; i++)
            {
                string sub = text.Substring(0, i);
                if (WorldText) WorldText.text = sub;
                yield return new WaitForSeconds(TypeSpeed);
            }

            yield return new WaitForSeconds(DisplayDuration);
            
            if (WorldText) WorldText.text = "";
        }
    }
}
