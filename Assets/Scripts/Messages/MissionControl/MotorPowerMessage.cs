/// <summary>
/// Message sent from Mission Control to the rover for setting a motor's power.
/// </summary>
public class MotorPowerMessage
{
    public string type = "motorPower";
    public string motor;
    public float power;
}
