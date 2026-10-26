using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [Header("Characteristics")]
    public float Health = 100f;
    public float Speed = 8f;

    [Header("Attack Settings")]
    public float AttackRadius = 3f;
    public float AttackDamage = 10f;
    public float AttackCooldown = 0.5f;
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
        AddComponent(entity, new AttackRadius
        {
            Value = authoring.AttackRadius,
            Damage = authoring.AttackDamage,
            Cooldown = authoring.AttackCooldown,
            Timer = 0f
        });

        AddComponent<PlayerTag>(entity);
    }
}
