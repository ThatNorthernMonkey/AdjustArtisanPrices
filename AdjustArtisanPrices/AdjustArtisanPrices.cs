using System;
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


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            PlayerEvents.InventoryChanged += this.PlayerEvents_OnInventoryChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player's inventory changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void PlayerEvents_OnInventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            if (!Context.IsWorldReady)
                return;

            Dictionary<string, int> parsedObjs = new Dictionary<string, int>();

            foreach (var entry in Game1.objectInformation)
            {
                var fields = entry.Value.Split('/');

                if (!parsedObjs.ContainsKey(fields[0]) && fields[3].Contains("Basic -75") || fields[3].Contains("Basic -79") || fields[3].Contains("Basic -26"))
                    parsedObjs.Add(fields[0], Convert.ToInt32(fields[1]));
            }

            foreach (SObject item in Game1.player.items.OfType<SObject>())
            {
                if (item.category != SObject.artisanGoodsCategory)
                    continue;

                string[] originalItemStringArray = item.Name.Split(' ');
                string originalItemName = "";
                string artisanType = "";

                if (originalItemStringArray.Any())
                {
                    if (originalItemStringArray.Length == 1)
                    {
                        artisanType = originalItemStringArray[0];
                    }

                    if (originalItemStringArray.Length == 2)
                    {
                        if (originalItemStringArray.Contains("Pale"))
                            artisanType = "Pale Ale";
                        else if (originalItemStringArray.Contains("Duck"))
                            artisanType = "Duck Mayonnaise";
                        else if (originalItemStringArray.Contains("Goat"))
                            artisanType = "Goat Cheese";
                        else
                        {
                            // E.g., Cranberries Wine
                            originalItemName = originalItemStringArray[0];
                            artisanType = originalItemStringArray[1];
                        }
                    }

                    //E.g., Hot Pepper Jelly
                    if (originalItemStringArray.Length == 3)
                    {
                        originalItemName += originalItemStringArray[0];
                        originalItemName += " ";
                        originalItemName += originalItemStringArray[1];

                        artisanType = originalItemStringArray[2];
                    }

                    var priceFromObjects = !originalItemName.Equals("")
                        ? parsedObjs.Select(x => x).FirstOrDefault(x => x.Key.Equals(originalItemName))
                        : parsedObjs.Select(x => x).FirstOrDefault(x => x.Key.Equals(artisanType));

                    int originalPrice = priceFromObjects.Value;

                    switch (artisanType)
                    {
                        case "Wine":
                            item.Price = (int)(originalPrice * Config.WineIncrease);
                            break;

                        case "Juice":
                            item.Price = (int)(originalPrice * Config.JuiceIncrease);
                            break;

                        case "Jelly":
                            item.Price = (int)(originalPrice * Config.JellyIncrease);
                            break;

                        case "Pickles":
                            item.Price = (int)(originalPrice * Config.PicklesIncrease);
                            break;

                        case "Pale Ale":
                            item.Price = (int)(originalPrice * Config.PaleAleIncrease);
                            break;

                        case "Beer":
                            item.Price = (int)(originalPrice * Config.BeerIncrease);
                            break;

                        case "Mayonnaise":
                            item.Price = (int)(originalPrice * Config.MayonnaiseIncrease);
                            break;

                        case "Duck Mayonnaise":
                            item.Price = (int)(originalPrice * Config.DuckMayonnaiseIncrease);
                            break;

                        case "Cheese":
                            item.Price = (int)(originalPrice * Config.CheeseIncrease);
                            break;

                        case "Goat Cheese":
                            item.Price = (int)(originalPrice * Config.GoatCheeseIncrease);
                            break;

                        case "Cloth":
                            item.Price = (int)(originalPrice * Config.ClothIncrease);
                            break;
                    }
                }
            }
        }
    }
}

