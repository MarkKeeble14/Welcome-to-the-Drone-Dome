public enum ModuleType
{
    NULL,
    AUTOMATIC_TURRET,
    BURST_FIRE_TURRET,
    SNIPER_TURRET,
    TESLA_COIL,
    EXPLOSIVE_SHELL_MORTAR,
    TOXIC_SHELL_MORTAR,
    DRONE_CONTACT_DAMAGE,
    DRONE_BLOCK_BULLETS,
    SHOCKWAVE_MORTAR,
    LASER_ACTIVE,
    ZAPPER,
    DEFAULT_TURRET,
    DRONE_BASICS,
    HELP_PLAYER_MOVEMENT
}

public static class EnumToStringHelper
{
    public static string GetStringValue(ModuleType type)
    {
        switch (type)
        {
            case ModuleType.NULL:
                return "null";
            case ModuleType.AUTOMATIC_TURRET:
                return "Automatic Turret";
            case ModuleType.BURST_FIRE_TURRET:
                return "Burst Fire Turret";
            case ModuleType.SNIPER_TURRET:
                return "Sniper Turret";
            case ModuleType.TESLA_COIL:
                return "Tesla Coil";
            case ModuleType.EXPLOSIVE_SHELL_MORTAR:
                return "Explosive Shell Mortar";
            case ModuleType.TOXIC_SHELL_MORTAR:
                return "Toxic Shell Mortar";
            case ModuleType.DRONE_CONTACT_DAMAGE:
                return "Drone Contact Damage";
            case ModuleType.DRONE_BLOCK_BULLETS:
                return "Drone Block Bullets";
            case ModuleType.SHOCKWAVE_MORTAR:
                return "Shockwave Mortar";
            case ModuleType.LASER_ACTIVE:
                return "Lasers";
            case ModuleType.ZAPPER:
                return "Zapper";
            case ModuleType.DEFAULT_TURRET:
                return "Built-In Turret";
            case ModuleType.DRONE_BASICS:
                return "Drone Basics";
            case ModuleType.HELP_PLAYER_MOVEMENT:
                return "Help Player Movement";
            default:
                return "Unmatched Module Type";
        }
    }

    public static string GetStringValue(UpgradeTreeRelation type)
    {
        switch (type)
        {
            case UpgradeTreeRelation.NULL:
                return "null";
            case UpgradeTreeRelation.AUTOMATIC_TURRET:
                return "Automatic Turret";
            case UpgradeTreeRelation.BURST_FIRE_TURRET:
                return "Burst Fire Turret";
            case UpgradeTreeRelation.SNIPER_TURRET:
                return "Sniper Turret";
            case UpgradeTreeRelation.TESLA_COIL:
                return "Tesla Coil";
            case UpgradeTreeRelation.EXPLOSIVE_SHELL_MORTAR:
                return "Explosive Shell Mortar";
            case UpgradeTreeRelation.TOXIC_SHELL_MORTAR:
                return "Toxic Shell Mortar";
            case UpgradeTreeRelation.DRONE_CONTACT_DAMAGE:
                return "Drone Contact Damage";
            case UpgradeTreeRelation.DRONE_BLOCK_BULLETS:
                return "Drone Block Bullets";
            case UpgradeTreeRelation.SHOCKWAVE_MORTAR:
                return "Shockwave Mortar";
            case UpgradeTreeRelation.LASER_ACTIVE:
                return "Lasers";
            case UpgradeTreeRelation.ZAPPER:
                return "Zapper";
            case UpgradeTreeRelation.DEFAULT_TURRET:
                return "Built-In Turret";
            case UpgradeTreeRelation.DRONE_BASICS:
                return "Drone Basics";
            case UpgradeTreeRelation.PLAYER_MOVEMENT:
                return "Player Movement";
            default:
                return "Unmatched Module Type";
        }
    }
}
