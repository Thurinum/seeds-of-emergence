using UnityEngine;

namespace GodBox.GameplayTags
{
    [CreateAssetMenu(fileName = "NewGameplayTag", menuName = "GodBox/Gameplay Tags/Tag")]
    public class GameplayTag : ScriptableObject
    {
        public string TagName;
        // Potential for parent tags here if needed later
        
        public bool Matches(GameplayTag other)
        {
            if (other == null) return false;
            return this == other; 
        }

        public override string ToString() => TagName;
    }
}
