namespace AdjustArtisanPrices.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The price multiplier for wines.</summary>
        public decimal WineIncrease { get; set; } = 1.75m;

        /// <summary>The price multiplier for juices.</summary>
        public decimal JuiceIncrease { get; set; } = 1.50m;

        /// <summary>The price multiplier for jellies.</summary>
        public decimal JellyIncrease { get; set; } = 1.25m;

        /// <summary>The price multiplier for pickles.</summary>
        public decimal PicklesIncrease { get; set; } = 1.25m;

        /// <summary>The price multiplier for pale ale.</summary>
        public decimal PaleAleIncrease { get; set; } = 0.50m;

        /// <summary>The price multiplier for beer.</summary>
        public decimal BeerIncrease { get; set; } = 0.50m;

        /// <summary>The price multiplier for mayonnaise.</summary>
        public decimal MayonnaiseIncrease { get; set; } = 0.75m;

        /// <summary>The price multiplier for duck mayonnaise.</summary>
        public decimal DuckMayonnaiseIncrease { get; set; } = 0.80m;

        /// <summary>The price multiplier for (cow) cheese.</summary>
        public decimal CheeseIncrease { get; set; } = 0.75m;

        /// <summary>The price multiplier for goat cheese.</summary>
        public decimal GoatCheeseIncrease { get; set; } = 0.75m;

        /// <summary>The price multiplier for cloth.</summary>
        public decimal ClothIncrease { get; set; } = 0.75m;
    }
}
