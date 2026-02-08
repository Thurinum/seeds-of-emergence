using UnityEngine;

namespace GodBox
{
    public class BasicNeedsComponent : MonoBehaviour
    {
        [Header("Food")]
        public float MaxFood = 100f;
        [SerializeField] private float _currentFood = 50f;
        public float FoodDecayRate = 1f; // per second

        [Header("Health")]
        public float MaxHealth = 100f;
        [SerializeField] private float _currentHealth = 100f;
        
        public float FoodPercentage => Mathf.Clamp01(_currentFood / MaxFood);
        public float HealthPercentage => Mathf.Clamp01(_currentHealth / MaxHealth);

        private void Update()
        {
            // Decay food
            if (_currentFood > 0)
            {
                _currentFood -= FoodDecayRate * Time.deltaTime;
                if (_currentFood < 0) _currentFood = 0;
            }
            else
            {
                // Starvation damage?
                TakeDamage(1f * Time.deltaTime);
            }
        }

        public void Eat(float amount)
        {
            _currentFood = Mathf.Min(_currentFood + amount, MaxFood);
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            _currentHealth = Mathf.Min(_currentHealth + amount, MaxHealth);
        }

        private void Die()
        {
            Debug.Log($"{name} has died.");
            // Handle death behavior
        }
    }
}
