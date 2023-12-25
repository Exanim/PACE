using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PACE.PaceMod.Recipes
{
    public class RecipeOverhaul : ModSystem
    {
        private readonly List<int> _oreList = new List<int>()
        {
            ItemID.CopperOre,
            ItemID.TinOre,
            ItemID.IronOre,
            ItemID.LeadOre,
            ItemID.SilverOre,
            ItemID.TungstenOre,
            ItemID.GoldOre,
            ItemID.PlatinumOre,
            ItemID.Meteorite,
            // ItemID.Hellstone,
            // ItemID.Obsidian,
            ItemID.CobaltOre,
            ItemID.PalladiumOre,
            ItemID.MythrilOre,
            ItemID.OrichalcumOre,

            ItemID.TitaniumOre,
            ItemID.AdamantiteOre,
            ItemID.ChlorophyteOre,
        };

        private readonly List<int> _barList = new List<int>()
        {
            ItemID.CopperBar,
            ItemID.TinBar,
            ItemID.IronBar,
            ItemID.LeadBar,
            ItemID.SilverBar,
            ItemID.TungstenBar,
            ItemID.GoldBar,
            ItemID.PlatinumBar,
            ItemID.Meteorite,
            // ItemID.Hellstone,
            // ItemID.Obsidian,
            ItemID.CobaltBar,
            ItemID.PalladiumBar,
            ItemID.MythrilBar,
            ItemID.OrichalcumBar,

            ItemID.TitaniumBar,
            ItemID.AdamantiteBar,
            ItemID.ChlorophyteBar,
        };


        public override void AddRecipes()
        {
            // TODO: disable decrafting for all bars that come from ores
            // you can craft anything that isn't a tool
            foreach (var recipe in Main.recipe)
            {
                if (OreToBarRecipe(recipe))
                {
                    recipe.DisableRecipe();
                }
            }
        }

        public override void PostAddRecipes()
        {

        }

        private bool OreToBarRecipe(Recipe recipe)
        {
            foreach (var ore in _oreList)
            {
                if (recipe.HasIngredient(ore))
                {
                    foreach (var bar in _barList)
                    {
                        if (recipe.HasResult(bar))
                        {
                            return true;
                            break;
                        }
                    }
                }
            }
            return false;
        }
    }
}
