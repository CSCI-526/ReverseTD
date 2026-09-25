using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// Fires projectiles at the target <see cref="TowerTargeting"/> picks, at the tower's fire rate.
    /// </summary>
    public class TowerWeapon : MonoBehaviour
    {
        private TowerDefinition definition;
        private TowerTargeting targeting;
        private float cooldown;

        /// <summary>Call once, right after adding the component.</summary>
        public void Initialize(TowerDefinition towerDefinition, TowerTargeting towerTargeting)
        {
            definition = towerDefinition;
            targeting = towerTargeting;
        }

        private void Update()
        {
            cooldown -= Time.deltaTime;
            if (cooldown > 0f)
            {
                return;
            }

            IDamageable target = targeting.FindTarget();
            if (target == null)
            {
                // Stay ready, so the tower fires the moment a soldier comes into range.
                cooldown = 0f;
                return;
            }

            Projectile.Launch(transform.position, target, definition.Damage, definition.ProjectileSpeed);

            // Adding keeps the leftover time, so the fire rate doesn't drift with the frame rate.
            cooldown += 1f / definition.FireRate;
        }
    }
}
