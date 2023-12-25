using PACE.Items.Tools;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PACE.PaceMod;

public class PacePlayer : ModPlayer
{

    public int CurrentlyViewedFactoryID = -1;

    public int CurrentlyViewedChargerID = -1;


    //Remove Vanilla's starting Items.
    public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> ItemsByMod, bool mediumCoreDeath)
    {
        ItemsByMod["Terraria"].RemoveAll((Item) => Item.type == ItemID.CopperAxe);
        ItemsByMod["Terraria"].RemoveAll((Item) => Item.type == ItemID.CopperPickaxe);
        ItemsByMod["Terraria"].RemoveAll((Item) => Item.type == ItemID.CopperShortsword);
    }

    //Add Arboreal Agitator
    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        return new Item[]
        {
            new Item(ModContent.ItemType<ArborealAgitator>(), 1, 0)
        };
    }
}
