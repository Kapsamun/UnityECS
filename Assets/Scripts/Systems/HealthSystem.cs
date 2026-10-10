using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MovementSystem))]
[BurstCompile]
public partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Health>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, entity) in
                 SystemAPI.Query<RefRO<Health>>()
                          .WithAll<EnemyTag>()
                          .WithNone<DeadTag>()
                          .WithEntityAccess())
        {
            if (health.ValueRO.Current <= 0f)
            {
                ecb.AddComponent<DeadTag>(entity);
                ecb.DestroyEntity(entity);
            }
        }
    }
}   
