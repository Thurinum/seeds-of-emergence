using System.Collections;
using UnityEngine;
using TMPro;

namespace GodBox.Conversation
{
    public class DialogueComponent : MonoBehaviour
    {
        public TextMeshPro WorldText; // For world space (overhead)
        public float TypeSpeed = 0.05f;
        public float DisplayDuration = 3f;

        private Coroutine _typeRoutine;

        public void Say(string text)
        {
            if (_typeRoutine != null) StopCoroutine(_typeRoutine);
            _typeRoutine = StartCoroutine(TypeWriter(text));
        }

        private IEnumerator TypeWriter(string text)
        {
            if (WorldText) WorldText.text = "";

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
