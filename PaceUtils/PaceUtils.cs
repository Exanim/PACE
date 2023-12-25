using Microsoft.Xna.Framework;
using PACE.Items;
using PACE.PaceMod;
using PACE.PaceMod.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PACE.PaceUtils;

public static class PaceUtils
{
    public static void SetMerge(int type1, int type2, bool merge = true)
    {
        if (type1 != type2)
        {
            Main.tileMerge[type1][type2] = merge;
            Main.tileMerge[type2][type1] = merge;
        }
    }
    public static Vector2 ToWorldCoordinates(Point p, float autoAddX = 8f, float autoAddY = 8f)
    {
        return p.ToVector2() * 16f + new Vector2(autoAddX, autoAddY);
    }

    public static PacePlayer Pace(this Player player)
    {
        return player.GetModPlayer<PacePlayer>();
    }

    public static PaceGlobalNPC Pace(this NPC npc)
    {
        return npc.GetGlobalNPC<PaceGlobalNPC>();
    }

    public static PaceGlobalItem Pace(this Item item)
    {
        return item.GetGlobalItem<PaceGlobalItem>();
    }

    public static T FindTileEntity<T>(int i, int j, int width, int height, int sheetSquare = 16) where T : ModTileEntity
    {
        Tile t = Main.tile[i, j];
        int left = i - t.TileFrameX % (width * sheetSquare) / sheetSquare;
        int top = j - t.TileFrameY % (height * sheetSquare) / sheetSquare;
        int chargerType = ((ModTileEntity)ModContent.GetInstance<T>()).Type;
        if (!TileEntity.ByPosition.TryGetValue(new Point16(left, top), out var te) || te.type != chargerType)
        {
            return default(T);
        }
        return (T)(object)te;
    }

    public static void CancelSignsAndChests(this Player player)
    {
        //IL_001e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0051: Unknown result type (might be due to invalid IL or missing references)
        Main.mouseRightRelease = false;
        if (player.sign >= 0)
        {
            SoundEngine.PlaySound(SoundID.MenuClose, null, null);
            player.sign = -1;
            Main.editSign = false;
            Main.npcChatText = "";
        }
        if (Main.editChest)
        {
            SoundEngine.PlaySound(SoundID.MenuTick, null, null);
            Main.editChest = false;
            Main.npcChatText = "";
        }
        if (player.chest >= 0)
        {
            player.chest = -1;
        }
    }

}