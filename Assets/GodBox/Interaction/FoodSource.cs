using System.Collections;
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
        public float RespawnTime = 0f;

        private Renderer[] _renderers;
        private Collider2D _collider;
        private GameplayTagComponent _tagComponent;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _collider = GetComponent<Collider2D>();
            _tagComponent = GetComponent<GameplayTagComponent>();
        }

        public override void Interact(GameObject interactor)
        {
            var needs = interactor.GetComponent<BasicNeedsComponent>();
            if (needs != null)
            {
                needs.Eat(FoodAmount);
                Debug.Log($"{interactor.name} ate {InteractionName}, restored {FoodAmount} food.");
                
                if (DestroyOnConsume)
                {
                    if (RespawnTime > 0)
                    {
                        StartCoroutine(RespawnRoutine());
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        private IEnumerator RespawnRoutine()
        {
            SetState(false);
            yield return new WaitForSeconds(RespawnTime);
            SetState(true);
        }

        private void SetState(bool active)
        {
            if (_collider) _collider.enabled = active;
            if (_tagComponent) _tagComponent.enabled = active; // Unregisters/Registers tag
            if (_renderers != null)
            {
                foreach (var r in _renderers) r.enabled = active;
            }
        }
    }
}
