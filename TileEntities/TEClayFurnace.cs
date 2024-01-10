using Microsoft.Xna.Framework;
using PACE.Items;
using PACE.PaceMod;
using PACE.Utils;
using PACE.Tiles;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PACE.TileEntities;

public class TEClayFurnace : ModTileEntity
{
    private short ChargingTimer;

    private float tolerance = 0.01f;

    private short _fuel;

    public Item ItemToSmelt = new Item();

    private bool syncItemCharge;

    public bool ClientChargingDust;

    public Vector2 Center => Utils.ToWorldCoordinates(Position, 24f, 16f);

    public short FuelAmount
    {
        get
        {
            return _fuel;
        }
        set
        {
            _fuel = value;
            SendSyncPacket();
        }
    }

    private bool CanSmeltItem
    {
        get
        {
            if (ItemToSmelt == null || ItemToSmelt.IsAir)
            {
                return false;
            }
            PaceGlobalItem modItem = ItemToSmelt.Pace();
            if (modItem.SmeltableByClayFurnace)
            {
                return modItem.Charge < modItem.MaxCharge;
            }
            return false;
        }
    }

    public bool CanDoWork
    {
        get
        {
            if (_fuel > 0)
            {
                return CanSmeltItem;
            }
            return false;
        }
    }

    public Color LightColor
    {
        get
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            if (!CanDoWork)
            {
                return Color.Red;
            }
            return Color.MediumSpringGreen;
        }
    }

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];

        if (tile.HasTile && tile.TileType == ModContent.TileType<ClayFurnace>() && tile.TileFrameX == 0)
        {
            return tile.TileFrameY == 0;
        }
        return false;
    }

    public override void Update()
    {
        if (!CanDoWork)
        {
            ChargingTimer = 0;
            return;
        }
        ChargingTimer++;
        if (ChargingTimer >= 8)
        {
            PaceGlobalItem modItem = ItemToSmelt.Pace();
            modItem.Charge += 1f;
            if (modItem.Charge >= modItem.MaxCharge)
            {
                modItem.Charge = modItem.MaxCharge;
            }
            SpawnChargingDust();
            syncItemCharge = true;
            FuelAmount--;
            ChargingTimer = 0;
        }
    }

    public void SpawnChargingDust()
    {
        bool chargeComplete = false;
        if (!ItemToSmelt.IsAir)
        {
            PaceGlobalItem modItem = ItemToSmelt.Pace();

            // TODO: test this line
            chargeComplete = Math.Abs(modItem.Charge - modItem.MaxCharge) < tolerance;
        }
        int dustID = 182;
        int numDust = 18;
        if (chargeComplete)
        {
            numDust *= 3;
        }
        Vector2 dustPos = Utils.ToWorldCoordinates(((TileEntity)this).Position, 20f, 1f);
        for (int i = 0; i < numDust; i += 2)
        {
            float pairSpeed = Utils.NextFloat(Main.rand, 0.5f, 7f);
            float pairScale = (chargeComplete ? 2.4f : 1f);
            Dust obj = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f, 0, default(Color), 1f);
            obj.velocity = Vector2.UnitX * pairSpeed;
            obj.scale = pairScale;
            obj.noGravity = true;
            Dust obj2 = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f, 0, default(Color), 1f);
            obj2.velocity = Vector2.UnitX * (0f - pairSpeed);
            obj2.scale = pairScale;
            obj2.noGravity = true;
        }
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
    {
        if (Main.netMode == 1)
        {
            NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, 2, (TileChangeType)0);
            NetMessage.SendData(87, -1, -1, (NetworkText)null, i, (float)j, (float)((ModTileEntity)this).Type, 0f, 0, 0, 0);
            return -1;
        }
        return ((ModTileEntity)this).Place(i, j);
    }

    public override void OnNetPlace()
    {
        NetMessage.SendData(86, -1, -1, (NetworkText)null, ((TileEntity)this).ID, (float)((TileEntity)this).Position.X, (float)((TileEntity)this).Position.Y, 0f, 0, 0, 0);
    }

    public override void OnKill()
    {
        for (int i = 0; i < 255; i++)
        {
            Player p = Main.player[i];
            if (p.active && ((ModPlayer[])typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p)).Length != 0)
            {
                PacePlayer mp = p.Pace();
                if (mp.CurrentlyViewedChargerID == ((TileEntity)this).ID)
                {
                    mp.CurrentlyViewedChargerID = -1;
                }
            }
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("time", (object)ChargingTimer);
        tag.Add("cells", (object)_fuel);
        Item forSaving;
        if (ItemToSmelt == null)
        {
            forSaving = new Item();
            forSaving.TurnToAir(false);
        }
        else
        {
            forSaving = ItemToSmelt;
        }
        tag.Add("item", (object)forSaving);
    }

    public override void LoadData(TagCompound tag)
    {
        ChargingTimer = tag.GetShort("time");
        _fuel = tag.GetShort("cells");
        ItemToSmelt = ItemIO.Load(tag.GetCompound("item"));
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(ChargingTimer);
        writer.Write(_fuel);
        ItemIO.Send(ItemToSmelt, writer, true, false);
    }

    public override void NetReceive(BinaryReader reader)
    {
        ChargingTimer = reader.ReadInt16();
        _fuel = reader.ReadInt16();
        ItemToSmelt = ItemIO.Receive(reader, true, false);
    }

    private void SendSyncPacket()
    {
        if (Main.netMode != 0)
        {
            ModPacket packet = ((ModTileEntity)this).Mod.GetPacket(256);
            packet.Write((byte)19);
            packet.Write(((TileEntity)this).ID);
            packet.Write(ChargingTimer);
            packet.Write(_fuel);
            PaceGlobalItem modItem = (ItemToSmelt.IsAir ? null : ItemToSmelt.Pace());
            packet.Write((syncItemCharge && modItem != null) ? modItem.Charge : float.NaN);
            packet.Send(-1, -1);
            syncItemCharge = false;
        }
    }

    internal static bool ReadSyncPacket(Mod mod, BinaryReader reader)
    {
        int teID = reader.ReadInt32();
        TileEntity te;
        bool num = TileEntity.ByID.TryGetValue(teID, out te);
        short timer = reader.ReadInt16();
        short cellStack = reader.ReadInt16();
        float chargeOrNaN = reader.ReadSingle();
        if (Main.netMode == 2)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write(19);
            packet.Write(teID);
            packet.Write(timer);
            packet.Write(cellStack);
            packet.Write(chargeOrNaN);
            packet.Send(-1, -1);
        }
        if (num && te is TEClayFurnace charger)
        {
            if (Main.netMode == 1)
            {
                charger.ChargingTimer = timer;
            }
            charger._fuel = cellStack;
            if (!float.IsNaN(chargeOrNaN))
            {
                PaceGlobalItem modItem = ((charger.ItemToSmelt != null && !charger.ItemToSmelt.IsAir) ? charger.ItemToSmelt.Pace() : null);
                if (modItem != null && modItem.UsesCharge)
                {
                    if (modItem.Charge != chargeOrNaN && Main.netMode == 1)
                    {
                        charger.ClientChargingDust = true;
                    }
                    modItem.Charge = chargeOrNaN;
                }
            }
            return true;
        }
        return false;
    }

    internal void SendItemSyncPacket()
    {
        if (Main.netMode != 0)
        {
            ModPacket packet = ((ModTileEntity)this).Mod.GetPacket(1024);
            packet.Write((byte)20);
            packet.Write(((TileEntity)this).ID);
            ItemIO.Send(ItemToSmelt, (BinaryWriter)(object)packet, true, false);
            packet.Send(-1, -1);
        }
    }

    internal static bool ReadItemSyncPacket(Mod mod, BinaryReader reader)
    {
        int teID = reader.ReadInt32();
        TileEntity te;
        bool num = TileEntity.ByID.TryGetValue(teID, out te);
        Item thePlug = ItemIO.Receive(reader, true, false);
        if (Main.netMode == 2)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)20);
            packet.Write(teID);
            ItemIO.Send(thePlug, (BinaryWriter)(object)packet, true, false);
            packet.Send(-1, -1);
        }
        if (num && te is TEClayFurnace charger)
        {
            charger.ItemToSmelt = thePlug;
            return true;
        }
        return false;
    }
}
