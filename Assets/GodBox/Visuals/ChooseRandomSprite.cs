using UnityEngine;

namespace GodBox.Visuals
{
    public class ChooseRandomSprite : MonoBehaviour
    {
        public Sprite[] Sprites;

        private void Start()
        {
            if (Sprites.Length == 0) return;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                int index = Random.Range(0, Sprites.Length);
                spriteRenderer.sprite = Sprites[index];
            }
        }
    }
}