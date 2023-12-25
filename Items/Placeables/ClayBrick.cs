using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PACE.Items.Placeables;

public class ClayBrick : ModItem
{

    public override void SetDefaults()
    {
        ((ModItem)this).Item.useStyle = 1;
        ((ModItem)this).Item.useTurn = true;
        ((ModItem)this).Item.useAnimation = 15;
        ((ModItem)this).Item.useTime = 10;
        ((ModItem)this).Item.autoReuse = true;
        ((ModItem)this).Item.consumable = true;
        ((Entity)((ModItem)this).Item).width = 16;
        ((Entity)((ModItem)this).Item).height = 16;
        ((ModItem)this).Item.maxStack = 9999;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.ClayBlock, 4)
            .AddCondition(Condition.NearWater)
            .Register();
    }
}
