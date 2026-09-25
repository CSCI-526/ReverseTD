using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// An HP bar: a dark background with a fill that shrinks toward the left and turns from green to red.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        private const float Height = 0.12f;

        private static readonly Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        private static readonly Color fullColor = new Color(0.35f, 0.85f, 0.4f);
        private static readonly Color emptyColor = new Color(0.9f, 0.25f, 0.2f);

        private SpriteRenderer fillRenderer;
        private Transform fill;
        private float width;

        /// <summary>Builds the bar, full. Call once, right after adding the component.</summary>
        /// <param name="barWidth">Width in world units.</param>
        public void Initialize(float barWidth)
        {
            width = barWidth;
            CreatePart("Background", backgroundColor, SortingOrders.HealthBar).transform.localScale = new Vector3(width, Height, 1f);
            fillRenderer = CreatePart("Fill", fullColor, SortingOrders.HealthBarFill);
            fill = fillRenderer.transform;
            SetFraction(1f);
        }

        /// <summary>Shows how full the bar is, from 0 (empty) to 1 (full). Other values are clamped.</summary>
        public void SetFraction(float fraction)
        {
            fraction = Mathf.Clamp01(fraction);
            fill.localScale = new Vector3(width * fraction, Height, 1f);
            // Keep the fill's left edge on the background's left edge.
            fill.localPosition = new Vector3((fraction - 1f) * width * 0.5f, 0f, 0f);
            fillRenderer.color = Color.Lerp(emptyColor, fullColor, fraction);
        }

        private SpriteRenderer CreatePart(string partName, Color color, int sortingOrder)
        {
            var part = new GameObject(partName);
            part.transform.SetParent(transform, false);

            var partRenderer = part.AddComponent<SpriteRenderer>();
            partRenderer.sprite = ShapeSprites.Square;
            partRenderer.color = color;
            partRenderer.sortingOrder = sortingOrder;
            return partRenderer;
        }
    }
}
