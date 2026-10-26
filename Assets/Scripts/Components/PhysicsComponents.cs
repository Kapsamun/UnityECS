using Unity.Entities;

public struct AttackRadius : IComponentData
{
    public float Value;
    public float Damage;
    public float Cooldown;
    public float Timer;
}