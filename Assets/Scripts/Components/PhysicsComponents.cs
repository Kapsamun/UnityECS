using Unity.Entities;

public struct AttackRadius : IComponentData
{
    public float Value; //радиус сферы поиска врагов
    public float Damage;
    public float Cooldown;
    public float Timer;
}

public struct ContactDamage : IComponentData
{
    public float Value; //урон в секунду при контакте
}