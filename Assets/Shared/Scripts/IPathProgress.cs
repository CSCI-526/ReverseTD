namespace ReverseTD.Shared
{
    /// <summary>
    /// Optional, for units that walk the stage path. Towers use it to target the unit
    /// furthest along the path; units without it are targeted by distance instead.
    /// </summary>
    public interface IPathProgress
    {
        /// <summary>Distance walked along the path from its start, in world units.</summary>
        float DistanceTraveled { get; }
    }
}
