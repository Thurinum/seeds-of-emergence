using GodBox;
using GodBox.GameplayTags;
using UnityEngine;

namespace GodBox.Interaction
{
    [RequireComponent(typeof(GameplayTagComponent))]
    public class FoodSource : Interactable
    {
        public float FoodAmount = 20f;
        public bool DestroyOnConsume = true;

        public override void Interact(GameObject interactor)
        {
            var needs = interactor.GetComponent<BasicNeedsComponent>();
            if (needs != null)
            {
                needs.Eat(FoodAmount);
                Debug.Log($"{interactor.name} ate {InteractionName}, restored {FoodAmount} food.");
                
                if (DestroyOnConsume)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
