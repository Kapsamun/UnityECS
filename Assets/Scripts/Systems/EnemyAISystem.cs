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

        float3 playerPos = SystemAPI.GetComponentRO<LocalTransform>(playerEntity).ValueRO.Position;

        var job = new EnemyAIJob
        {
            PlayerPosition = playerPos
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
[WithAll(typeof(EnemyTag))]
[WithNone(typeof(DeadTag))]
public partial struct EnemyAIJob : IJobEntity
{
    public float3 PlayerPosition;

    public void Execute(
        ref TargetPosition target,
        ref Velocity velocity,
        in LocalTransform transform,
        in MoveSpeed speed)
    {
        target.Value = PlayerPosition;

        float3 direction = PlayerPosition - transform.Position;
        direction.y = 0f;

        if (math.lengthsq(direction) > 0.01f)
        {
            direction = math.normalize(direction);
            velocity.Value = direction * speed.Value;
        }
        else
        {
            velocity.Value = float3.zero;
        }
    }
}