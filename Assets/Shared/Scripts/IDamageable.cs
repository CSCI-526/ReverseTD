using UnityEngine;

namespace ReverseTD.Shared
{
    /// <summary>
    /// A combat unit that can be targeted and damaged: soldiers (<see cref="Team.Attacker"/>)
    /// and towers (<see cref="Team.Defender"/>).
    /// Register live units with <see cref="CombatRegistry"/> so the other side can find them.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>The side this unit fights for.</summary>
        Team Team { get; }

        /// <summary>False once the unit has died. Dead units are never targeted.</summary>
        bool IsAlive { get; }

        /// <summary>World position, used for range checks and aiming.</summary>
        Vector3 Position { get; }

        /// <summary>Applies damage. Does nothing if the unit is already dead.</summary>
        /// <param name="amount">Damage to apply. Positive.</param>
        void TakeDamage(float amount);
    }
}
