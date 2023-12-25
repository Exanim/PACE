using PACE.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace PACE.Items.Placeables.Machines;

public class ClayFurnaceItem : ModItem
{
    public override void SetDefaults()
    {
        Entity.width = 26;
        Entity.height = 26;
        Item.maxStack = 9999;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = Item.useTime = 15;
        Item.useStyle = 1;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<ClayFurnace>();
        Item.value = Item.buyPrice(0, 50, 0, 0);
    }
}