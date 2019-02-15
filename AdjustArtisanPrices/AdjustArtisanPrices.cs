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
        private ModConfig Config;

        /// <summary>The base prices for items by ID.</summary>
        private readonly IDictionary<int, int> ItemPrices = new Dictionary<int, int>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after items are added or removed to a player's inventory.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsLocalPlayer)
                return;

            foreach (SObject item in Game1.player.Items.OfType<SObject>())
            {
                if (item.Category != SObject.artisanGoodsCategory)
                    continue;

                // get price info
                int ingredientPrice = this.GetIngredientPrice(item);
                decimal multiplier = this.GetMultiplier(item);
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
                    return this.Config.JellyIncrease;

                case SObject.PreserveType.Juice:
                    return this.Config.JuiceIncrease;

                case SObject.PreserveType.Pickle:
                    return this.Config.PicklesIncrease;

                case Object.PreserveType.Wine:
                    return this.Config.WineIncrease;
            }

            // by item ID
            switch (item.ParentSheetIndex)
            {
                case 303:
                    return this.Config.PaleAleIncrease;

                case 346:
                    return this.Config.BeerIncrease;

                case 306:
                    return this.Config.MayonnaiseIncrease;

                case 307:
                    return this.Config.DuckMayonnaiseIncrease;

                case 424:
                    return this.Config.CheeseIncrease;

                case 426:
                    return this.Config.GoatCheeseIncrease;

                case 428:
                    return this.Config.ClothIncrease;
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
                return this.GetPriceOf(item.preservedParentSheetIndex.Value);

            // by item ID
            switch (item.ParentSheetIndex)
            {
                // hops => pale ale
                case 303:
                    return this.GetPriceOf(304);

                // wheat => beer
                case 346:
                    return this.GetPriceOf(262);

                // egg => normal mayonnaise
                case 306:
                    return this.GetPriceOf(176);

                // duck egg => duck mayonnaise
                case 307:
                    return this.GetPriceOf(442);

                // milk => cheese
                case 424:
                    return this.GetPriceOf(184);

                // goat milk => goat cheese
                case 426:
                    return this.GetPriceOf(436);

                // wool => cloth
                case 428:
                    return this.GetPriceOf(440);
            }

            return -1;
        }

        /// <summary>Get the price of the given item.</summary>
        /// <param name="itemID">The item ID.</param>
        private int GetPriceOf(int itemID)
        {
            // get cached price
            if (this.ItemPrices.ContainsKey(itemID))
                return this.ItemPrices[itemID];

            // get price
            SObject item = new SObject(itemID, 1);
            this.ItemPrices[item.ParentSheetIndex] = item.Price;
            return item.Price;
        }
    }
}

