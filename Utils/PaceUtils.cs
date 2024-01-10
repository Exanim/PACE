using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PACE.Items;
using PACE.Items.Placeables;
using PACE.PaceMod;
using PACE.PaceMod.NPCs;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace PACE.Utils;

public static class PaceUtils
{

    public static Rectangle MouseHitbox => new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

    internal static void DrawPowercellSlot(SpriteBatch spriteBatch, Item item, Vector2 drawPosition, float iconScale = 0.7f)
    {
        Texture2D slotBackgroundTex = ModContent.Request<Texture2D>("PACE/UI/PowerCellSlot_Empty").Value;
        if (item.stack > 0)
        {
            slotBackgroundTex = ((item.type != ModContent.ItemType<SteelWorkBench>()) ?
                ModContent.Request<Texture2D>("PACE/UI/PowerCellSlot_Filled").Value :
                ModContent.Request<Texture2D>("PACE/UI/PowerCellSlot_Blood").Value);
        }
        spriteBatch.Draw(slotBackgroundTex, drawPosition, null, Color.White, 0f, Terraria.Utils.Size(slotBackgroundTex) * 0.5f, iconScale, 0, 0f);
        if (item.stack > 0)
        {
            float inventoryScale = Main.inventoryScale * iconScale;
            Vector2 numberOffset = Terraria.Utils.Size(slotBackgroundTex) * 0.2f;
            numberOffset.X -= 17f;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), drawPosition + numberOffset * inventoryScale, Color.White, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
        }
    }

    public static void SetMerge(int type1, int type2, bool merge = true)
    {
        if (type1 != type2)
        {
            Main.tileMerge[type1][type2] = merge;
            Main.tileMerge[type2][type1] = merge;
        }
    }
    public static Vector2 ToWorldCoordinates(Point16 p, float autoAddX = 8f, float autoAddY = 8f)
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