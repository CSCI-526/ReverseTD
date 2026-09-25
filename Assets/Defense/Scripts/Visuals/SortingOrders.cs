namespace ReverseTD.Defense
{
    /// <summary>
    /// SpriteRenderer.sortingOrder values, drawn lowest first. Everything stays on the Default
    /// sorting layer, so no custom sorting layers are needed.
    /// </summary>
    public static class SortingOrders
    {
        public const int Path = 0;
        public const int RangeIndicator = 10;
        public const int Tower = 20;
        public const int Soldier = 30;
        public const int Projectile = 40;
        public const int HealthBar = 50;
        public const int HealthBarFill = 51;
    }
}
