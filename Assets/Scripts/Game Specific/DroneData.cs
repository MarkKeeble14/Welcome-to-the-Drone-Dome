public struct DroneData
{
    public DroneController DroneController;
    public DroneAttackTargeting DroneTargeting;

    public DroneData(DroneController x, DroneAttackTargeting y)
    {
        DroneController = x;
        DroneTargeting = y;
    }
}
