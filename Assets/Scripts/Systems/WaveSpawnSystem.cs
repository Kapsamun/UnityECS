using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Спавн волн из Blob через EntityCommandBuffer.
/// Префаб берётся из Singleton PrefabsHolder.
/// Позиции — случайные (Unity.Mathematics.Random).
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

        // ECB — безопасное создание Entity
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Singleton: префабы
        var prefabs = SystemAPI.GetSingleton<PrefabsHolder>();

        // Singleton: Blob с волнами
        var wavesHolder = SystemAPI.GetSingleton<WavesHolder>();
        ref var wavesBlob = ref wavesHolder.WavesReference.Value;

        // Позиция игрока — центр спавна
        float3 center = float3.zero;
        if (SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
        {
            center = SystemAPI.GetComponentRO<LocalTransform>(playerEntity).ValueRO.Position;
        }

        // Random (сид от времени)
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

            // Ждём паузу перед волной
            if (waveState.ValueRO.IsWaiting)
            {
                waveState.ValueRW.Timer += dt;
                if (waveState.ValueRO.Timer < currentWave.WaitTimeBeforeWave)
                    continue;

                // Пауза закончилась — спавним
                waveState.ValueRW.IsWaiting = false;
                waveState.ValueRW.Timer = 0f;
            }

            // Спавн всех групп врагов текущей волны
            for (int e = 0; e < currentWave.Enemies.Length; e++)
            {
                var enemyGroup = currentWave.Enemies[e];

                for (int i = 0; i < enemyGroup.Amount; i++)
                {
                    // Пока один префаб; EnemyType можно использовать для выбора разных префабов
                    Entity enemy = ecb.Instantiate(prefabs.EnemyPrefab);

                    // Случайная позиция на окружности (пункт 3 — Random)
                    float angle = random.NextFloat(0f, math.PI * 2f);
                    float radius = waveState.ValueRO.SpawnRadius;
                    float3 offset = new float3(math.cos(angle), 0f, math.sin(angle)) * radius;
                    float3 spawnPos = center + offset;

                    // Небольшой разброс
                    spawnPos += random.NextFloat3Direction() * random.NextFloat(0f, 3f);
                    spawnPos.y = 1f;

                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
                }
            }

            // Переход к следующей волне
            waveState.ValueRW.CurrentWaveIndex++;
            waveState.ValueRW.IsWaiting = true;
            waveState.ValueRW.Timer = 0f;
        }
    }
}
