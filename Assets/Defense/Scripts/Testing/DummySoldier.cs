// TEST ONLY: a stand-in soldier for testing towers until teammates' soldiers exist.

using System.Collections.Generic;
using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense.Testing
{
    /// <summary>
    /// A circle that walks the stage path, can be shot, and hits towers it passes. It darkens as it loses HP.
    /// </summary>
    public class DummySoldier : MonoBehaviour, IDamageable, IPathProgress
    {
        private const float Size = 0.4f;
        private const float AttackInterval = 1f; // seconds between attacks

        private static readonly Color fullHealthColor = new Color(0.95f, 0.35f, 0.3f);
        private static readonly Color lowHealthColor = new Color(0.35f, 0.08f, 0.08f);

        [SerializeField, Tooltip("Hit points at spawn. Set by the spawner.")]
        private float maxHp;

        [SerializeField, Tooltip("Current hit points.")]
        private float hp;

        [SerializeField, Tooltip("Walking speed, in world units per second. Set by the spawner.")]
        private float speed;

        [SerializeField, Tooltip("Damage per attack on a tower. Set by the spawner.")]
        private float damage;

        [SerializeField, Tooltip("How close a tower must be to attack it, in world units. Set by the spawner.")]
        private float attackRange;

        private readonly List<IDamageable> candidates = new List<IDamageable>();
        private StagePath path;
        private SpriteRenderer body;
        private float distanceTraveled;
        private float attackCooldown;
        private bool isAlive = true;

        public Team Team => Team.Attacker;
        public bool IsAlive => isAlive;
        public Vector3 Position => transform.position;
        public float DistanceTraveled => distanceTraveled;

        /// <summary>Call once while the object is inactive, then activate it.</summary>
        public void Initialize(StagePath stagePath, float hitPoints, float walkSpeed, float attackDamage, float towerAttackRange)
        {
            path = stagePath;
            maxHp = hitPoints;
            hp = hitPoints;
            speed = walkSpeed;
            damage = attackDamage;
            attackRange = towerAttackRange;
            transform.position = path.Start;

            var bodyObject = new GameObject("Body");
            bodyObject.transform.SetParent(transform, false);
            bodyObject.transform.localScale = new Vector3(Size, Size, 1f);
            body = bodyObject.AddComponent<SpriteRenderer>();
            body.sprite = ShapeSprites.Circle;
            body.color = fullHealthColor;
            body.sortingOrder = SortingOrders.Soldier;
        }

        private void OnEnable()
        {
            CombatRegistry.Register(this);
        }

        private void OnDisable()
        {
            CombatRegistry.Unregister(this);
        }

        private void Update()
        {
            if (!isAlive)
            {
                return;
            }

            distanceTraveled += speed * Time.deltaTime;
            transform.position = path.GetPointAtDistance(distanceTraveled);

            // Broke through. What that means is up to the real game; the dummy just leaves.
            if (distanceTraveled >= path.TotalLength)
            {
                Despawn();
                return;
            }

            Attack();
        }

        public void TakeDamage(float amount)
        {
            if (!isAlive)
            {
                return;
            }

            hp -= amount;
            body.color = Color.Lerp(lowHealthColor, fullHealthColor, hp / maxHp);
            if (hp <= 0f)
            {
                Despawn();
            }
        }

        // Hits the nearest tower in range once per AttackInterval, without stopping.
        private void Attack()
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown > 0f)
            {
                return;
            }

            IDamageable target = FindNearestTower();
            if (target == null)
            {
                // Stay ready, so the dummy hits a tower the moment one comes into range.
                attackCooldown = 0f;
                return;
            }

            target.TakeDamage(damage);
            attackCooldown += AttackInterval;
        }

        private IDamageable FindNearestTower()
        {
            CombatRegistry.GetAlive(Team.Defender, candidates);

            Vector2 origin = transform.position;
            float nearestSqr = attackRange * attackRange;
            IDamageable nearest = null;
            for (int i = 0; i < candidates.Count; i++)
            {
                float distanceSqr = ((Vector2)candidates[i].Position - origin).sqrMagnitude;
                if (distanceSqr <= nearestSqr)
                {
                    nearest = candidates[i];
                    nearestSqr = distanceSqr;
                }
            }
            return nearest;
        }

        // Marked dead right away, so nothing targets it before Destroy runs at the end of the frame.
        private void Despawn()
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }
}
