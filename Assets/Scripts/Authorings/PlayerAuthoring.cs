using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [Header("Characteristics")]
    public float Health = 100f;
    public float Speed = 8f;

    [Header("Attack Settings")]
    public float AttackRadius = 4f;   // Радиус сферы атаки
    public float AttackDamage = 35f;  // Урон (у врага 30 HP, погибнет с 1 удара)
    public float AttackCooldown = 0.3f;
}

public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Health
        {
            Current = authoring.Health,
            Max = authoring.Health
        });
        AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
        AddComponent(entity, new Velocity { Value = float3.zero });

        // Защита от нулевых значений из инспектора
        float radius = authoring.AttackRadius > 0 ? authoring.AttackRadius : 4f;
        float damage = authoring.AttackDamage > 0 ? authoring.AttackDamage : 35f;
        float cooldown = authoring.AttackCooldown > 0 ? authoring.AttackCooldown : 0.3f;

        AddComponent(entity, new AttackRadius
        {
            Value = radius,
            Damage = damage,
            Cooldown = cooldown,
            Timer = 0f
        });

        AddComponent<PlayerTag>(entity);
    }
}