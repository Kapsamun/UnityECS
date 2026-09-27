using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Header("Health")]
    public float Health = 30f;

    [Header("Movement")]
    public float Speed = 3.5f;

    [Header("Stamina")]
    public float StaminaMax = 50f;
    public float StaminaRegenRate = 8f;
    public float StaminaDrainRate = 12f;
}

public class EnemyAuthoringBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Health
        {
            Current = authoring.Health,
            Max = authoring.Health
        });

        AddComponent(entity, new Stamina
        {
            Current = authoring.StaminaMax,
            Max = authoring.StaminaMax,
            RegenRate = authoring.StaminaRegenRate,
            DrainRate = authoring.StaminaDrainRate
        });

        AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
        AddComponent(entity, new Velocity { Value = float3.zero });
        AddComponent(entity, new TargetPosition { Value = float3.zero });
        AddComponent<EnemyTag>(entity);
    }
}