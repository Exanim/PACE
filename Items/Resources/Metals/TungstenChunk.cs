using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace PACE.Items.Resources.Metals;

public class TungstenChunk : ModItem
{
    public override void SetDefaults()
    {
        this.Item.useStyle = 1;
        this.Item.useTurn = true;
        this.Item.useAnimation = 15;
        this.Item.useTime = 10;
        this.Item.autoReuse = true;
        this.Item.consumable = true;
        this.Item.width = 16;
        this.Item.height = 16;
        this.Item.maxStack = 9999;
    }
}
