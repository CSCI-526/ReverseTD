using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// A tower's hit points, and the target soldiers attack (<see cref="Team.Defender"/>).
    /// At 0 HP it tells its <see cref="Tower"/> to die.
    /// </summary>
    public class TowerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField, Tooltip("Current hit points. Starts at the definition's max HP.")]
        private float hp;

        private Tower tower;
        private TowerDefinition definition;
        private HealthBar healthBar;
        private bool isAlive = true;

        public Team Team => Team.Defender;
        public bool IsAlive => isAlive;
        public Vector3 Position => transform.position;

        /// <summary>Call once, right after adding the component.</summary>
        public void Initialize(Tower owner, TowerDefinition towerDefinition, HealthBar bar)
        {
            tower = owner;
            definition = towerDefinition;
            healthBar = bar;
            hp = definition.MaxHp;
        }

        private void OnEnable()
        {
            CombatRegistry.Register(this);
        }

        private void OnDisable()
        {
            CombatRegistry.Unregister(this);
        }

        public void TakeDamage(float amount)
        {
            if (!isAlive)
            {
                return;
            }

            hp = Mathf.Max(0f, hp - amount);
            // MaxHp is read live, so tuning it during Play shows up on the next hit.
            healthBar.SetFraction(hp / definition.MaxHp);
            if (hp <= 0f)
            {
                // Marked dead first, so nothing targets it or kills it a second time.
                isAlive = false;
                tower.Die();
            }
        }
    }
}
