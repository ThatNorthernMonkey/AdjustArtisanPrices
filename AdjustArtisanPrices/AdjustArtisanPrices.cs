using System.Collections.Generic;
using System.Linq;
using AdjustArtisanPrices.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace AdjustArtisanPrices
{
    /// <summary>The main entry class.</summary>
    public class AdjustArtisanPrices : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig _config;

        /// <summary>The base prices for items by ID.</summary>
        private readonly IDictionary<int, int> _itemPrices = new Dictionary<int, int>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.Player.InventoryChanged += PlayerEvents_OnInventoryChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player's inventory changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void PlayerEvents_OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            foreach (SObject item in Game1.player.Items.OfType<SObject>())
            {
                if (item.Category != SObject.artisanGoodsCategory)
                    continue;

                // get price info
                int ingredientPrice = GetIngredientPrice(item);
                decimal multiplier = GetMultiplier(item);
                if (ingredientPrice <= 0 || multiplier == 1)
                    continue;

                // recalculate prices
                item.Price = (int)(ingredientPrice * multiplier);
            }
        }

        /// <summary>Get the price multiplier for an artisanal item.</summary>
        /// <param name="item">The artisanal item.</param>
        private decimal GetMultiplier(SObject item)
        {
            // by preserve type
            switch (item.preserve.Value)
            {
                case SObject.PreserveType.Jelly:
                    return _config.JellyIncrease;

                case SObject.PreserveType.Juice:
                    return _config.JuiceIncrease;

                case SObject.PreserveType.Pickle:
                    return _config.PicklesIncrease;

                case Object.PreserveType.Wine:
                    return _config.WineIncrease;
            }

            // by item ID
            switch (item.ParentSheetIndex)
            {
                case 303:
                    return _config.PaleAleIncrease;

                case 346:
                    return _config.BeerIncrease;

                case 306:
                    return _config.MayonnaiseIncrease;

                case 307:
                    return _config.DuckMayonnaiseIncrease;

                case 424:
                    return _config.CheeseIncrease;

                case 426:
                    return _config.GoatCheeseIncrease;

                case 428:
                    return _config.ClothIncrease;
            }

            // unknown
            return 1;
        }

        /// <summary>Get the price of the item used to create the artisanal item.</summary>
        /// <param name="item">The artisanal item.</param>
        /// <returns>Returns the ingredient price, or -1 if none found.</returns>
        private int GetIngredientPrice(SObject item)
        {
            // get preserved item
            if (item.preservedParentSheetIndex.Value != 0)
                return GetPriceOf(item.preservedParentSheetIndex.Value);

            // by item ID
            switch (item.ParentSheetIndex)
            {
                // hops => pale ale
                case 303:
                    return GetPriceOf(304);

                // wheat => beer
                case 346:
                    return GetPriceOf(262);

                // egg => normal mayonnaise
                case 306:
                    return GetPriceOf(176);

                // duck egg => duck mayonnaise
                case 307:
                    return GetPriceOf(442);

                // milk => cheese
                case 424:
                    return GetPriceOf(184);

                // goat milk => goat cheese
                case 426:
                    return GetPriceOf(436);

                // wool => cloth
                case 428:
                    return GetPriceOf(440);
            }

            return -1;
        }

        /// <summary>Get the price of the given item.</summary>
        /// <param name="itemId">The item ID.</param>
        private int GetPriceOf(int itemId)
        {
            // get cached price
            if (_itemPrices.ContainsKey(itemId))
                return _itemPrices[itemId];

            // get price
            SObject item = new SObject(itemId, 1);
            _itemPrices[item.ParentSheetIndex] = item.Price;
            return item.Price;
        }
    }
}
