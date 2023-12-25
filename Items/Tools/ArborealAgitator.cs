using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PACE.Items.Tools;


internal class ArborealAgitator : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;

        Item.useTime = 100;
        Item.useAnimation = 15;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;

        Item.DamageType = DamageClass.Melee;
        Item.damage = 1;
        Item.knockBack = 100f;

        Item.value = Item.buyPrice(copper: 1);
        Item.rare = ItemRarityID.Blue;
        Item.axe = 1;
    }


}
