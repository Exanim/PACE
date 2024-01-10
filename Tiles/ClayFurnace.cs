using Microsoft.Xna.Framework;
using System;
using PACE.Items.Placeables.Machines;
using PACE.PaceMod;
using PACE.Utils;
using PACE.TileEntities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace PACE.Tiles;

public class ClayFurnace : ModTile
{
    public const int Width = 3;

    public const int Height = 2;

    public const int OriginOffsetX = 1;

    public const int OriginOffsetY = 1;

    public const int SheetSquare = 18;

    public const int FramesPerChargeAction = 8;

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileWaterDeath[Type] = false;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.LavaDeath = false;
        ModTileEntity te = ModContent.GetInstance<TEClayFurnace>();
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);
        TileObjectData.addTile(Type);
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226, 0f, 0f, 0, default(Color), 1f);
        return false;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = (fail ? 1 : 3);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        int itemType = ModContent.ItemType<ClayFurnaceItem>();

        Item.NewItem(
            new EntitySource_TileBreak(i, j),
            i * 16,
            j * 16,
            Width * 16,
            Height * 16,
            itemType
        );
    }

    public override bool RightClick(int i, int j)
    {
        TEClayFurnace thisCharger = Utils.PaceUtils.FindTileEntity<TEClayFurnace>(i, j, 3, 2, 18);
        Player localPlayer = Main.LocalPlayer;
        localPlayer.CancelSignsAndChests();
        PacePlayer mp = localPlayer.Pace();
        if (thisCharger == null || (thisCharger).ID == mp.CurrentlyViewedChargerID)
        {
            mp.CurrentlyViewedChargerID = -1;
            SoundEngine.PlaySound(SoundID.MenuClose, null, null);
        }
        else if (thisCharger != null)
        {
            SoundStyle val = ((mp.CurrentlyViewedChargerID == -1) ? SoundID.MenuOpen : SoundID.MenuTick);
            SoundEngine.PlaySound(val, null, null);
            mp.CurrentlyViewedChargerID = (thisCharger).ID;
            Main.playerInventory = true;
            Main.recBigList = false;
        }
        Recipe.FindRecipes(false);
        return true;
    }

}
