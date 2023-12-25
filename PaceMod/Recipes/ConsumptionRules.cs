using Terraria;
using Terraria.ID;

namespace PACE.PaceMod.Recipes
{
    public static class ConsumptionRules
    {
        // call when creating a recipe for an item see: https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-item-consumption
        public static void DontConsumeChain(Recipe recipe, int type, ref int amount)
        {
            if (type == ItemID.Chain)
            {
                amount = 0;
            }
        }

        // Other ConsumeItemCallback delegates can go here.
    }
}
