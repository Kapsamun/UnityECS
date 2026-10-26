using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MovementSystem))]
[BurstCompile]
public partial struct HealthSystem : ISystem
{
    private EntityQuery _query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder()
            .WithAll<Health, EnemyTag>()
            .WithNone<DeadTag>()
            .Build();

        state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new HealthJob
        {
            ECB = ecb
        };

        state.Dependency = job.ScheduleParallel(_query, state.Dependency);
    }
}

[BurstCompile]
public partial struct HealthJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Health health)
    {
        if (health.Current <= 0f)
        {
            ECB.AddComponent<DeadTag>(chunkIndex, entity);
            ECB.DestroyEntity(chunkIndex, entity);
        }
    }
}