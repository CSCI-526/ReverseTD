using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// A tower on the board, built from a <see cref="TowerDefinition"/>.
    /// <see cref="BoardView"/> spawns it inactive, calls <see cref="Initialize"/>, then activates it.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        private const float HealthBarGap = 0.2f; // between the body's top edge and the HP bar
        private const float HealthBarExtraWidth = 0.2f;

        [SerializeField, Tooltip("Stats and look. Set by BoardView when the tower spawns.")]
        private TowerDefinition definition;

        private BoardView board;

        /// <summary>
        /// Raised when a tower's HP reaches 0, just before it's removed. Hook rewards in here.
        /// The tower's name, position, and definition are still valid during the call.
        /// </summary>
        public static event System.Action<Tower> Destroyed;

        public TowerDefinition Definition => definition;

        // Domain reload is disabled, so statics (event subscribers included) survive between Play sessions unless cleared here.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Destroyed = null;
        }

        /// <summary>
        /// Sets the tower's definition and adds its parts: body, range ring, HP bar, health, targeting, and weapon.
        /// Call once, right after spawning. Stats are read from the definition while playing, so edits
        /// to it show up right away; the body's size and color are applied only here.
        /// </summary>
        public void Initialize(TowerDefinition towerDefinition, BoardView owner)
        {
            definition = towerDefinition;
            board = owner;

            CreateBody();
            CreateChild("Range").AddComponent<RangeIndicator>().Initialize(definition);

            GameObject barObject = CreateChild("HP Bar");
            barObject.transform.localPosition = new Vector3(0f, definition.Size * 0.5f + HealthBarGap, 0f);
            var healthBar = barObject.AddComponent<HealthBar>();
            healthBar.Initialize(definition.Size + HealthBarExtraWidth);
            gameObject.AddComponent<TowerHealth>().Initialize(this, definition, healthBar);

            var targeting = gameObject.AddComponent<TowerTargeting>();
            targeting.Initialize(definition);
            gameObject.AddComponent<TowerWeapon>().Initialize(definition, targeting);
        }

        // Called by TowerHealth at 0 HP.
        internal void Die()
        {
            // Destroy takes effect at the end of the frame, so handlers still see a valid tower.
            // Scheduling it first means a handler that throws can't leave a dead tower on screen.
            Destroy(gameObject);
            try
            {
                Destroyed?.Invoke(this);
            }
            finally
            {
                // After every Destroyed handler, so AllTowersDestroyed always comes last.
                if (board != null)
                {
                    board.HandleTowerDestroyed(this);
                }
            }
        }

        private void CreateBody()
        {
            // The body is a child, so its scale doesn't affect the other children, like the range ring.
            GameObject body = CreateChild("Body");
            body.transform.localScale = new Vector3(definition.Size, definition.Size, 1f);

            var bodyRenderer = body.AddComponent<SpriteRenderer>();
            bodyRenderer.sprite = ShapeSprites.Square;
            bodyRenderer.color = definition.Color;
            bodyRenderer.sortingOrder = SortingOrders.Tower;
        }

        private GameObject CreateChild(string childName)
        {
            var child = new GameObject(childName);
            child.transform.SetParent(transform, false);
            return child;
        }
    }
}
