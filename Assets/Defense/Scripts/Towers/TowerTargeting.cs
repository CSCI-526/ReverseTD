using System.Collections.Generic;
using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// Picks the soldier a tower shoots, using the tower's range and <see cref="TargetingMode"/>.
    /// </summary>
    public class TowerTargeting : MonoBehaviour
    {
        private readonly List<IDamageable> candidates = new List<IDamageable>();
        private TowerDefinition definition;

        /// <summary>Call once, right after adding the component.</summary>
        public void Initialize(TowerDefinition towerDefinition)
        {
            definition = towerDefinition;
        }

        /// <summary>The live soldier in range to shoot, or null if there's none.</summary>
        public IDamageable FindTarget()
        {
            CombatRegistry.GetAlive(Team.Attacker, candidates);

            Vector2 origin = transform.position;
            float rangeSqr = definition.Range * definition.Range;
            IDamageable nearest = null;
            float nearestSqr = float.MaxValue;
            IDamageable first = null;
            float firstProgress = float.MinValue;

            for (int i = 0; i < candidates.Count; i++)
            {
                IDamageable candidate = candidates[i];
                float distanceSqr = ((Vector2)candidate.Position - origin).sqrMagnitude;
                if (distanceSqr > rangeSqr)
                {
                    continue;
                }

                if (distanceSqr < nearestSqr)
                {
                    nearest = candidate;
                    nearestSqr = distanceSqr;
                }

                if (candidate is IPathProgress progress && progress.DistanceTraveled > firstProgress)
                {
                    first = candidate;
                    firstProgress = progress.DistanceTraveled;
                }
            }

            // First falls back to nearest when no soldier in range reports path progress.
            if (definition.TargetingMode == TargetingMode.First && first != null)
            {
                return first;
            }
            return nearest;
        }
    }
}
