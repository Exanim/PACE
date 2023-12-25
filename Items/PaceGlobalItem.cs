using Terraria.ModLoader;

namespace PACE.Items;
public class PaceGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public bool SmeltableByClayFurnace = false;

    public float Charge;

    public float MaxCharge = 1f;

    public bool UsesCharge;
}
