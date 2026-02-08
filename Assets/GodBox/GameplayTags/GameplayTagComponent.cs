using System.Collections.Generic;
using UnityEngine;

namespace GodBox.GameplayTags
{
    public class GameplayTagComponent : MonoBehaviour
    {
        public List<GameplayTag> Tags = new List<GameplayTag>();

        private void OnEnable()
        {
            foreach (var tag in Tags)
            {
                GameplayTagManager.Instance.Register(tag, gameObject);
            }
        }

        private void OnDisable()
        {
            // Check instance existence to avoid errors on quitting
            // Note: In real prod code, handle this more gracefully
            foreach (var tag in Tags)
            {
                GameplayTagManager.Instance?.Unregister(tag, gameObject);
            }
        }
        
        public bool HasTag(GameplayTag tag)
        {
            return Tags.Contains(tag);
        }
    }
}
