using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Система работает только когда есть спавнер
        state.RequireForUpdate<EnemySpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Используем ECB из BeginSimulation — безопасно и работает с Burst
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Позиция игрока (центр спавна)
        float3 center = float3.zero;
        if (SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
        {
            center = SystemAPI.GetComponentRO<LocalTransform>(playerEntity).ValueRO.Position;
        }

        // Random (один на кадр)
        uint seed = (uint)(SystemAPI.Time.ElapsedTime * 1000.0 + 1);
        var random = new Random(seed);

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            spawner.ValueRW.Timer += dt;

            if (spawner.ValueRO.Timer < spawner.ValueRO.Interval)
                continue;

            // Сбрасываем таймер
            spawner.ValueRW.Timer = 0f;

            // Спавним m врагов
            for (int i = 0; i < spawner.ValueRO.Count; i++)
            {
                Entity enemy = ecb.Instantiate(spawner.ValueRO.Prefab);

                // Случайная точка на окружности
                float angle = random.NextFloat(0f, math.PI * 2f);
                float3 offset = new float3(math.cos(angle), 0f, math.sin(angle)) * spawner.ValueRO.Radius;
                float3 spawnPos = center + offset;

                // Немного рандома по радиусу (чтобы не было идеального круга)
                spawnPos += random.NextFloat3Direction() * random.NextFloat(0f, 3f);
                spawnPos.y = 1;

                ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
            }
        }
    }
}