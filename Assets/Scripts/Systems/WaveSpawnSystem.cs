using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Спавн волн из Blob через EntityCommandBuffer.
/// Префаб берётся из Singleton PrefabsHolder.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct WaveSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WavesHolder>();
        state.RequireForUpdate<PrefabsHolder>();
        state.RequireForUpdate<WaveState>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var prefabs = SystemAPI.GetSingleton<PrefabsHolder>();

        var wavesHolder = SystemAPI.GetSingleton<WavesHolder>();
        ref var wavesBlob = ref wavesHolder.WavesReference.Value;

        float3 center = float3.zero;
        if (SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
        {
            center = SystemAPI.GetComponentRO<LocalTransform>(playerEntity).ValueRO.Position;
        }

        uint seed = (uint)(SystemAPI.Time.ElapsedTime * 1000.0 + 1);
        var random = new Random(seed);

        foreach (var waveState in SystemAPI.Query<RefRW<WaveState>>())
        {
            if (waveState.ValueRO.AllWavesDone)
                continue;

            int waveIndex = waveState.ValueRO.CurrentWaveIndex;

            // Все волны пройдены
            if (waveIndex >= wavesBlob.Waves.Length)
            {
                waveState.ValueRW.AllWavesDone = true;
                continue;
            }

            ref var currentWave = ref wavesBlob.Waves[waveIndex];

            if (waveState.ValueRO.IsWaiting)
            {
                waveState.ValueRW.Timer += dt;
                if (waveState.ValueRO.Timer < currentWave.WaitTimeBeforeWave)
                    continue;

                waveState.ValueRW.IsWaiting = false;
                waveState.ValueRW.Timer = 0f;
            }

            for (int e = 0; e < currentWave.Enemies.Length; e++)
            {
                var enemyGroup = currentWave.Enemies[e];

                for (int i = 0; i < enemyGroup.Amount; i++)
                {
                    Entity enemy = ecb.Instantiate(prefabs.EnemyPrefab);

                    // Случайная позиция на окружности 
                    float angle = random.NextFloat(0f, math.PI * 2f);
                    float radius = waveState.ValueRO.SpawnRadius;
                    float3 offset = new float3(math.cos(angle), 0f, math.sin(angle)) * radius;
                    float3 spawnPos = center + offset;

                   
                    spawnPos += random.NextFloat3Direction() * random.NextFloat(0f, 3f);
                    spawnPos.y = 1f;

                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
                }
            }

            waveState.ValueRW.CurrentWaveIndex++;
            waveState.ValueRW.IsWaiting = true;
            waveState.ValueRW.Timer = 0f;
        }
    }
}
