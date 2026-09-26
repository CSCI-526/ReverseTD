using System.Collections.Generic;
using UnityEngine;
using ReverseTD.Shared;
using ReverseTD.Defense;

namespace ReverseTD.Offense
{
    public class OffenseSoldier : MonoBehaviour, IDamageable, IPathProgress
    {
        private const float Size = 0.4f;
        private const float AttackInterval = 1f;

        private static readonly Color fullHealthColor = new Color(0.95f, 0.35f, 0.3f);
        private static readonly Color lowHealthColor = new Color(0.35f, 0.08f, 0.08f);

        [SerializeField] private float maxHp;
        [SerializeField] private float hp;
        [SerializeField] private float speed;
        [SerializeField] private float damage;
        [SerializeField] private float attackRange;

        private readonly List<IDamageable> candidates = new List<IDamageable>();
        private StagePath path;
        private SpriteRenderer body;
        private OffenseData offenseData;
        private float distanceTraveled;
        private float attackCooldown;
        private bool isAlive = true;

        // Shared Contract Implementation
        public Team Team => Team.Attacker;
        public bool IsAlive => isAlive;
        public Vector3 Position => transform.position;
        public float DistanceTraveled => distanceTraveled;

        public void Initialize(StagePath stagePath, OffenseData data, float towerAttackRange = 1.5f)
        {
            path = stagePath;
            offenseData = data;

            // Pull values dynamically from persistent upgrade data
            maxHp = data != null ? data.MaxHealth : 5f;
            hp = maxHp;
            speed = 1.0f;
            damage = data != null ? data.AttackDamage : 1f;
            attackRange = towerAttackRange;

            transform.position = path.Start;

            // Create visual circle sprite
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
            if (!isAlive || path == null) return;

            distanceTraveled += speed * Time.deltaTime;
            transform.position = path.GetPointAtDistance(distanceTraveled);

            // Reached the end of the track
            if (distanceTraveled >= path.TotalLength)
            {
                Despawn();
                return;
            }

            Attack();
        }

        public void TakeDamage(float amount)
        {
            if (!isAlive) return;

            hp -= amount;
            if (body != null)
            {
                body.color = Color.Lerp(lowHealthColor, fullHealthColor, hp / maxHp);
            }

            if (hp <= 0f)
            {
                Despawn();
            }
        }

        private void Attack()
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown > 0f) return;

            IDamageable target = FindNearestTower();
            if (target == null)
            {
                attackCooldown = 0f;
                return;
            }

            target.TakeDamage(damage);
            attackCooldown += AttackInterval;

            // Incremental reward: grant gold whenever this unit deals damage to a tower
            if (offenseData != null)
            {
                offenseData.gold += damage;
            }
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

        private void Despawn()
        {
            isAlive = false;

            // Notify the wave manager that this unit is off the board
            if (OffenseWaveManager.Instance != null)
            {
                OffenseWaveManager.Instance.ReportSoldierDeath();
            }

            Destroy(gameObject);
        }
    }
}