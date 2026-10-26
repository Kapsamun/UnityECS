using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Header("Characteristics")]
    public float Health = 30f;
    public float Speed = 3.5f;

    [Header("Contact Damage Settings")]
    public float ContactDamage = 5f;
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
        AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
        AddComponent(entity, new Velocity { Value = float3.zero });
        AddComponent(entity, new TargetPosition { Value = float3.zero });
        AddComponent(entity, new ContactDamage
        {
            Value = authoring.ContactDamage
        });

        AddComponent<EnemyTag>(entity);
    }
}
