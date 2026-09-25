using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// A homing shot. It flies at its target and damages it on arrival. If the target dies first,
    /// it finishes the flight to the target's last position and vanishes without doing damage.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private const float Size = 0.2f;
        private const float HitDistance = 0.05f;
        private const float MaxLifetime = 5f; // a safety net for shots that never arrive

        private static readonly Color projectileColor = new Color(0.6f, 0.95f, 1f);

        private IDamageable target;
        private Vector3 aimPoint;
        private float damage;
        private float speed;
        private float age;

        /// <summary>Spawns a projectile at <paramref name="origin"/> that homes in on a live <paramref name="target"/>.</summary>
        public static Projectile Launch(Vector3 origin, IDamageable target, float damage, float speed)
        {
            var projectileObject = new GameObject("Projectile");
            projectileObject.transform.position = origin;
            projectileObject.transform.localScale = new Vector3(Size, Size, 1f);

            var spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = ShapeSprites.Circle;
            spriteRenderer.color = projectileColor;
            spriteRenderer.sortingOrder = SortingOrders.Projectile;

            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.target = target;
            // Aim now: the first Update runs a frame later, and the target may die before then.
            projectile.aimPoint = target.Position;
            projectile.damage = damage;
            projectile.speed = speed;
            return projectile;
        }

        private void Update()
        {
            bool targetAlive = CombatRegistry.IsAlive(target);
            if (targetAlive)
            {
                aimPoint = target.Position;
            }

            transform.position = Vector3.MoveTowards(transform.position, aimPoint, speed * Time.deltaTime);
            if ((transform.position - aimPoint).sqrMagnitude <= HitDistance * HitDistance)
            {
                if (targetAlive)
                {
                    target.TakeDamage(damage);
                }
                Destroy(gameObject);
                return;
            }

            age += Time.deltaTime;
            if (age >= MaxLifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
