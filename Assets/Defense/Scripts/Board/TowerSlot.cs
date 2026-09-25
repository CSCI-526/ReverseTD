using System;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>A place on the board where a tower stands.</summary>
    [Serializable]
    public struct TowerSlot
    {
        [SerializeField, Tooltip("Tower center, in world units.")]
        private Vector2 position;

        [SerializeField, Tooltip("The tower that stands here.")]
        private TowerDefinition definition;

        public Vector2 Position => position;
        public TowerDefinition Definition => definition;
    }
}
