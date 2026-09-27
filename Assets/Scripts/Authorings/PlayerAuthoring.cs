using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [Header("Health")]
    public float Health = 100f;

    [Header("Movement")]
    public float Speed = 8f;

    [Header("Stamina")]
    public float StaminaMax = 100f;
    public float StaminaRegenRate = 15f;
    public float StaminaDrainRate = 25f;
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

        AddComponent(entity, new Stamina
        {
            Current = authoring.StaminaMax,
            Max = authoring.StaminaMax,
            RegenRate = authoring.StaminaRegenRate,
            DrainRate = authoring.StaminaDrainRate
        });

        AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
        AddComponent(entity, new Velocity { Value = float3.zero });
        AddComponent<PlayerTag>(entity);
    }
}