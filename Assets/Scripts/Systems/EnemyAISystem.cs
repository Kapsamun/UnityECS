using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(MovementSystem))]
[BurstCompile]
public partial struct EnemyAISystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
            return;

        var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);
        float3 playerPos = playerTransform.ValueRO.Position;

        foreach (var (target, transform, speed, velocity) in
                 SystemAPI.Query<RefRW<TargetPosition>, RefRO<LocalTransform>,
                                 RefRO<MoveSpeed>, RefRW<Velocity>>()
                          .WithAll<EnemyTag>())
        {
            target.ValueRW.Value = playerPos;

            float3 direction = playerPos - transform.ValueRO.Position;
            direction.y = 0;

            if (math.lengthsq(direction) > 0.01f)
            {
                direction = math.normalize(direction);
                velocity.ValueRW.Value = direction * speed.ValueRO.Value;
            }
            else
            {
                velocity.ValueRW.Value = float3.zero;
            }
        }
    }
}