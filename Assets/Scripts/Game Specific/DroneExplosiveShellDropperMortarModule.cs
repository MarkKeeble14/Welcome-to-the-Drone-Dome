public class DroneExplosiveShellDropperMortarModule : DroneMortarModule
{
    public override ModuleType Type => ModuleType.EXPLOSIVE_DROPPER_SHELL_MORTAR;
    protected override WeaponTargetingType TargetBy
    {
        get
        {
            return WeaponTargetingType.FURTHEST;
        }
    }
}

