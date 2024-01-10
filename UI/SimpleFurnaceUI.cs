using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PACE.Items.Resources.Metals;
using PACE.PaceMod;
using PACE.Utils;
using PACE.TileEntities;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;


namespace PACE.UI;

public class SimpleFurnaceUI
{
    public const float MaxPlayerDistance = 160f;

    private const float IconScale = 0.7f;

    private const int GuiWidth = 36;

    private const int GuiHeight = 36;

    private const int SlotSpacing = 8;

    private const float SlotDrawOffsetX = 24f;

    private const float CellDrawOffsetY = -20f;

    private const float PluggedDrawOffsetY = -64f;

    public static void Draw(SpriteBatch spriteBatch)
    {
        Player p = Main.LocalPlayer;
        PacePlayer mp = p.Pace();
        int chargerID = mp.CurrentlyViewedChargerID;
        if (chargerID == -1)
        {
            return;
        }
        TEClayFurnace charger;
        bool syncRequired;
        short fuelAmount;
        bool shiftClicked;
        if (TileEntity.ByID.TryGetValue(chargerID, out var te) && te is TEClayFurnace cast)
        {
            charger = cast;
            if (!Main.playerInventory || p.chest != -1)
            {
                mp.CurrentlyViewedChargerID = -1;
                return;
            }
            Vector2 chargerWorldCenter = charger.Center;
            if (p.DistanceSQ(chargerWorldCenter) > 25600f)
            {
                SoundEngine.PlaySound(SoundID.MenuClose, null, null);
                mp.CurrentlyViewedChargerID = -1;
                return;
            }
            int powercellID = ModContent.ItemType<CopperChunk>();
            ref Item pluggedItem = ref charger.ItemToSmelt;
            Item powercell = new Item();
            powercell.TurnToAir(false);
            if (charger.FuelAmount > 0 || powercell.maxStack == 0)
            {
                powercell.SetDefaults(powercellID);
                powercell.stack = charger.FuelAmount;
            }
            Vector2 val = PaceUtils.ToWorldCoordinates(charger.Position, 0f, 0f);
            DrawWeaponSlot(drawPosition: val + new Vector2(24f, -64f) - Main.screenPosition, spriteBatch: spriteBatch, item: pluggedItem);
            Vector2 powercellDrawPos = val + new Vector2(24f, -20f) - Main.screenPosition;
            PaceUtils.DrawPowercellSlot(spriteBatch, powercell, powercellDrawPos);
            Rectangle mouseRect = PaceUtils.MouseHitbox;

            int slotRectX = (int)(val.X - 1f);
            int pluggedSlotRectY = (int)(val.Y + -64f - 18f);
            Rectangle pluggedSlotRect = new Rectangle(slotRectX, pluggedSlotRectY, 36, 36);

            int cellSlotRectY = (int)(val.Y + -20f - 18f);
            Rectangle powercellSlotRect = new Rectangle(slotRectX, pluggedSlotRectY, 36, 36);
            Player.ItemSpaceStatus val2;
            if (mouseRect.Intersects(pluggedSlotRect))
            {
                p.mouseInterface = (Main.blockMouse = true);
                if (!pluggedItem.IsAir)
                {
                    Main.HoverItem = pluggedItem.Clone();
                }
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    syncRequired = false;
                    if (Terraria.Utils.PressingShift(Main.keyState))
                    {
                        val2 = p.ItemSpace(pluggedItem);
                        if (((Player.ItemSpaceStatus)(ref val2)).CanTakeItemToPersonalInventory)
                        {
                            p.QuickSpawnItem((IEntitySource)new EntitySource_TileEntity((TileEntity)(object)charger, (string)null), pluggedItem, pluggedItem.stack);
                            pluggedItem.TurnToAir(false);
                            if (!Main.mouseItem.IsAir && Main.mouseItem.Calamity().UsesCharge)
                            {
                                Utils.Swap<Item>(ref Main.mouseItem, ref pluggedItem);
                            }
                            syncRequired = true;
                            goto IL_02ab;
                        }
                    }
                    if ((Main.mouseItem.IsAir && !pluggedItem.IsAir) || (!Main.mouseItem.IsAir && Main.mouseItem.Calamity().UsesCharge))
                    {
                        Utils.Swap<Item>(ref Main.mouseItem, ref pluggedItem);
                        SoundEngine.PlaySound(ref SoundID.Grab, (Vector2?)null, (SoundUpdateCallback)null);
                        syncRequired = true;
                    }
                    goto IL_02ab;
                }
                goto IL_02b5;
            }
            if (!mouseRect.Intersects(powercellSlotRect))
            {
                return;
            }
            if (!powercell.IsAir)
            {
                Main.HoverItem = powercell;
            }
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                fuelAmount = 0;
                shiftClicked = false;
                if (Utils.PressingShift(Main.keyState))
                {
                    val2 = p.ItemSpace(powercell);
                    if (((IL_Player.ItemSpaceStatus)(ref val2)).CanTakeItemToPersonalInventory)
                    {
                        p.QuickSpawnItem(((Entity)p).GetSource_TileInteraction((int)te.Position.X, (int)te.Position.Y, (string)null), powercellID, powercell.stack);
                        fuelAmount = (short)(-powercell.stack);
                        shiftClicked = true;
                        goto IL_040f;
                    }
                }
                if (Main.mouseItem.type == powercellID && powercell.stack < powercell.maxStack)
                {
                    int spaceLeft = powercell.maxStack - powercell.stack;
                    int cellsToInsert = Math.Min(Main.mouseItem.stack, spaceLeft);
                    fuelAmount = (short)cellsToInsert;
                    Item mouseItem = Main.mouseItem;
                    mouseItem.stack -= cellsToInsert;
                    if (Main.mouseItem.stack == 0)
                    {
                        Main.mouseItem.TurnToAir(false);
                    }
                }
                else if (Main.mouseItem.IsAir && powercell.stack > 0)
                {
                    fuelAmount = (short)(-powercell.stack);
                    Main.mouseItem.SetDefaults(powercell.type);
                    Main.mouseItem.stack = powercell.stack;
                    powercell.TurnToAir(false);
                }
                goto IL_040f;
            }
            goto IL_043d;
        }
        mp.CurrentlyViewedChargerID = -1;
        return;
    IL_02b5:
        Main.instance.MouseTextHackZoom("", null);
        return;
    IL_040f:
        if (fuelAmount != 0)
        {
            if (!shiftClicked)
            {
                SoundEngine.PlaySound(ref SoundID.Grab, (Vector2?)null, (SoundUpdateCallback)null);
            }
            charger.FuelAmount += fuelAmount;
        }
        goto IL_043d;
    IL_043d:
        Main.instance.MouseTextHackZoom("", null);
        Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0;
        return;
    IL_02ab:
        if (syncRequired)
        {
            charger.SendItemSyncPacket();
        }
        goto IL_02b5;
    }

    public static void DrawWeaponSlot(SpriteBatch spriteBatch, Item item, Vector2 drawPosition)
    {
        Texture2D slotBackgroundTex = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonsArsenal/ChargerWeaponSlot", (AssetRequestMode)2).Value;
        spriteBatch.Draw(slotBackgroundTex, drawPosition, (Rectangle?)null, Color.White, 0f, Terraria.Utils.Size(slotBackgroundTex) * 0.5f, 0.7f, (SpriteEffects)0, 0f);
        if (!item.IsAir)
        {
            float inventoryScale = Main.inventoryScale;
            Texture2D itemTexture = TextureAssets.Item[item.type].Value;
            Rectangle itemFrame = ((Main.itemAnimations[item.type] == null) ? Terraria.Utils.Frame(itemTexture, 1, 1, 0, 0, 0, 0) : Main.itemAnimations[item.type].GetFrame(itemTexture, -1));
            float baseScale = 1f;
            Color _ = Color.White;
            ItemSlot.GetItemLight(ref _, ref baseScale, item, false);
            float scaleRestrictor = 1f;
            if (itemFrame.Width > 46 || itemFrame.Height > 46)
            {
                int restrictingDim = Math.Max(itemFrame.Width, itemFrame.Height);
                scaleRestrictor = 46f / (float)restrictingDim;
            }
            scaleRestrictor *= inventoryScale;
            if (ItemLoader.PreDrawInInventory(item, spriteBatch, drawPosition, itemFrame, item.GetAlpha(Color.White), item.GetColor(Color.White), Terraria.Utils.Size(itemTexture) * 0.5f, scaleRestrictor * baseScale))
            {
                spriteBatch.Draw(itemTexture, drawPosition, (Rectangle?)itemFrame, item.GetAlpha(Color.White), 0f, Terraria.Utils.Size(itemTexture) * 0.5f, scaleRestrictor * baseScale, (SpriteEffects)0, 0f);
                spriteBatch.Draw(itemTexture, drawPosition, (Rectangle?)itemFrame, item.GetColor(Color.White), 0f, Terraria.Utils.Size(itemTexture) * 0.5f, scaleRestrictor * baseScale, (SpriteEffects)0, 0f);
            }
        }
    }
}
