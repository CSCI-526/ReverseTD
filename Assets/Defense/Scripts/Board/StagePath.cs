using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// The path soldiers walk, from <see cref="Start"/> to <see cref="End"/>.
    /// Get the stage's path from <see cref="BoardView.Path"/>.
    /// </summary>
    /// <remarks>
    /// Movement code keeps a distance along the path and asks for the matching point:
    /// <code>
    /// distance += speed * Time.deltaTime;
    /// transform.position = path.GetPointAtDistance(distance);
    /// bool brokeThrough = distance >= path.TotalLength;
    /// </code>
    /// </remarks>
    public sealed class StagePath
    {
        private readonly Vector2[] waypoints;
        private readonly float[] distanceAtWaypoint;

        /// <summary>Builds a path through the given points.</summary>
        /// <param name="points">Path corners in walking order, in world coordinates. Needs at least 2.</param>
        /// <exception cref="ArgumentException">There are fewer than 2 points.</exception>
        public StagePath(IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count < 2)
            {
                throw new ArgumentException("A stage path needs at least 2 waypoints.", nameof(points));
            }

            waypoints = new Vector2[points.Count];
            distanceAtWaypoint = new float[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                waypoints[i] = points[i];
                if (i > 0)
                {
                    distanceAtWaypoint[i] = distanceAtWaypoint[i - 1] + Vector2.Distance(waypoints[i - 1], waypoints[i]);
                }
            }

            TotalLength = distanceAtWaypoint[distanceAtWaypoint.Length - 1];
        }

        /// <summary>Path corners in walking order, in world coordinates.</summary>
        public IReadOnlyList<Vector2> Waypoints => waypoints;

        /// <summary>Length of the whole path, in world units.</summary>
        public float TotalLength { get; }

        /// <summary>Where soldiers enter: the first waypoint.</summary>
        public Vector2 Start => waypoints[0];

        /// <summary>Where soldiers break through: the last waypoint.</summary>
        public Vector2 End => waypoints[waypoints.Length - 1];

        /// <summary>The point at a given distance along the path from <see cref="Start"/>.</summary>
        /// <param name="distance">World units from the start. Clamped to 0..<see cref="TotalLength"/>.</param>
        public Vector2 GetPointAtDistance(float distance)
        {
            if (distance <= 0f)
            {
                return Start;
            }

            for (int i = 1; i < waypoints.Length; i++)
            {
                if (distance <= distanceAtWaypoint[i])
                {
                    float segmentLength = distanceAtWaypoint[i] - distanceAtWaypoint[i - 1];
                    if (segmentLength <= 0f)
                    {
                        return waypoints[i];
                    }

                    float t = (distance - distanceAtWaypoint[i - 1]) / segmentLength;
                    return Vector2.Lerp(waypoints[i - 1], waypoints[i], t);
                }
            }

            return End;
        }
    }
}
