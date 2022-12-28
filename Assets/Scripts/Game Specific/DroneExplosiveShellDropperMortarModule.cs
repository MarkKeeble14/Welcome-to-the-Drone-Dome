public class DroneExplosiveShellDropperMortarModule : DroneMortarModule
{

    protected override WeaponTargetingType TargetBy
    {
        get
        {
            return WeaponTargetingType.FURTHEST;
        }
    }
}

