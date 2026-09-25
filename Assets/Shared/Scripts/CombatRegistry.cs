using System.Collections.Generic;
using UnityEngine;

namespace ReverseTD.Shared
{
    /// <summary>
    /// Lists every live combat unit by team, so units find targets without colliders, tags, or layers.
    /// </summary>
    /// <remarks>
    /// Units call <see cref="Register"/> in <c>OnEnable</c> and <see cref="Unregister"/> in <c>OnDisable</c>.
    /// </remarks>
    public static class CombatRegistry
    {
        private static readonly List<IDamageable> attackers = new List<IDamageable>();
        private static readonly List<IDamageable> defenders = new List<IDamageable>();

        // Cached so GetAlive doesn't allocate a delegate per call.
        private static readonly System.Predicate<IDamageable> isDestroyed = IsDestroyed;

        // Domain reload is disabled, so statics survive between Play sessions unless cleared here.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            attackers.Clear();
            defenders.Clear();
        }

        /// <summary>
        /// Adds a unit to its team's list. Registering the same unit twice has no effect.
        /// </summary>
        /// <param name="unit">The unit to add. Ignored if null.</param>
        public static void Register(IDamageable unit)
        {
            if (unit == null)
            {
                return;
            }

            List<IDamageable> list = ListFor(unit.Team);
            if (!list.Contains(unit))
            {
                list.Add(unit);
            }
        }

        /// <summary>
        /// Removes a unit. Safe to call for a unit that isn't registered.
        /// </summary>
        /// <param name="unit">The unit to remove. Ignored if null.</param>
        public static void Unregister(IDamageable unit)
        {
            if (unit == null)
            {
                return;
            }

            attackers.Remove(unit);
            defenders.Remove(unit);
        }

        /// <summary>
        /// Fills <paramref name="results"/> with the live units of a team.
        /// The list is cleared first, so reuse one list instead of allocating a new one per call.
        /// </summary>
        /// <param name="team">The team to list.</param>
        /// <param name="results">Receives the units. Must not be null.</param>
        public static void GetAlive(Team team, List<IDamageable> results)
        {
            results.Clear();

            List<IDamageable> list = ListFor(team);
            list.RemoveAll(isDestroyed);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive)
                {
                    results.Add(list[i]);
                }
            }
        }

        /// <summary>
        /// True if the unit still exists and is alive. Use it on units you keep a reference to:
        /// a plain <c>== null</c> check on an interface doesn't notice a destroyed Unity object.
        /// </summary>
        /// <param name="unit">The unit to check. May be null.</param>
        public static bool IsAlive(IDamageable unit)
        {
            return unit != null && !IsDestroyed(unit) && unit.IsAlive;
        }

        private static List<IDamageable> ListFor(Team team)
        {
            return team == Team.Attacker ? attackers : defenders;
        }

        // A destroyed component keeps its C# object; only Unity's == null notices it's gone.
        private static bool IsDestroyed(IDamageable unit)
        {
            return unit is Object unityObject && unityObject == null;
        }
    }
}
