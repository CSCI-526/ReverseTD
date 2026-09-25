using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// A faint ring showing a tower's attack range. It follows the <see cref="TowerDefinition"/>,
    /// so range and color edits made during Play show up right away.
    /// </summary>
    public class RangeIndicator : MonoBehaviour
    {
        private const float Alpha = 0.3f;

        private TowerDefinition definition;
        private SpriteRenderer ring;
        private float shownRange;
        private Color shownColor;

        /// <summary>Builds the ring. Call once, right after adding the component.</summary>
        public void Initialize(TowerDefinition towerDefinition)
        {
            definition = towerDefinition;
            ring = gameObject.AddComponent<SpriteRenderer>();
            ring.sprite = ShapeSprites.Ring;
            ring.sortingOrder = SortingOrders.RangeIndicator;
            Apply();
        }

        private void LateUpdate()
        {
            if (definition.Range != shownRange || definition.Color != shownColor)
            {
                Apply();
            }
        }

        private void Apply()
        {
            shownRange = definition.Range;
            shownColor = definition.Color;

            // The ring sprite is one unit across, so scale it to the range's diameter.
            float diameter = shownRange * 2f;
            transform.localScale = new Vector3(diameter, diameter, 1f);
            ring.color = new Color(shownColor.r, shownColor.g, shownColor.b, Alpha);
        }
    }
}
