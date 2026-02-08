using UnityEngine;

namespace GodBox.Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        public string InteractionName;
        public float InteractionRange = 2f;

        public abstract void Interact(GameObject interactor);
    }
}
