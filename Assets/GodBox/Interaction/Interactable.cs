using UnityEngine;

namespace GodBox.Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        public string InteractionName;
        public float InteractionRange = 1f;

        public abstract void Interact(GameObject interactor);
    }
}
