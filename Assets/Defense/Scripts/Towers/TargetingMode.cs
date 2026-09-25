namespace ReverseTD.Defense
{
    /// <summary>How a tower picks its target among the soldiers in range.</summary>
    public enum TargetingMode
    {
        /// <summary>The soldier furthest along the path. Falls back to nearest for soldiers without path progress.</summary>
        First,

        /// <summary>The soldier closest to the tower.</summary>
        Nearest
    }
}
