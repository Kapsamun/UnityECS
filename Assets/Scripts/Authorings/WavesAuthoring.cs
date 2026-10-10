using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


[Serializable]
public class EnemiesWave
{
    public float WaitTimeBeforeWave = 3f;
    public List<WaveEnemy> EnemiesInWave = new List<WaveEnemy>();
}

[Serializable]
public class WaveEnemy
{
    public EnemyType EnemyType = EnemyType.Basic;
    public int Amount = 10;
}

/// <summary>
/// Authoring волн. Данные из Inspector запекаются в Blob.
/// </summary>
public class WavesAuthoring : MonoBehaviour
{
    [Header("Волны противников")]
    public List<EnemiesWave> Waves = new List<EnemiesWave>();

    [Header("Настройки спавна")]
    public float SpawnRadius = 20f;
}

public class WavesAuthoringBaker : Baker<WavesAuthoring>
{
    public override void Bake(WavesAuthoring authoring)
    {
        // --- Создание Blob ---
        var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var root = ref blobBuilder.ConstructRoot<WavesBlob>();

        // Выделяем массив волн
        BlobBuilderArray<Wave> wavesArray =
            blobBuilder.Allocate(ref root.Waves, authoring.Waves.Count);

        for (int i = 0; i < authoring.Waves.Count; i++)
        {
            var srcWave = authoring.Waves[i];
            wavesArray[i].WaitTimeBeforeWave = srcWave.WaitTimeBeforeWave;

            // Выделяем массив врагов внутри волны
            BlobBuilderArray<EnemyInWave> enemiesArray =
                blobBuilder.Allocate(ref wavesArray[i].Enemies, srcWave.EnemiesInWave.Count);

            for (int j = 0; j < srcWave.EnemiesInWave.Count; j++)
            {
                enemiesArray[j] = new EnemyInWave
                {
                    EnemyType = srcWave.EnemiesInWave[j].EnemyType,
                    Amount = srcWave.EnemiesInWave[j].Amount
                };
            }
        }

        // Создаём постоянную ссылку на Blob
        BlobAssetReference<WavesBlob> blobRef =
            blobBuilder.CreateBlobAssetReference<WavesBlob>(Allocator.Persistent);

        blobBuilder.Dispose();

        // Регистрируем Blob (дедупликация + авто-освобождение памяти)
        AddBlobAsset(ref blobRef, out _);

        // --- Entity с WavesHolder + WaveState ---
        var entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new WavesHolder
        {
            WavesReference = blobRef
        });

        AddComponent(entity, new WaveState
        {
            CurrentWaveIndex = 0,
            Timer = 0f,
            IsWaiting = true,
            AllWavesDone = false,
            SpawnRadius = authoring.SpawnRadius
        });
    }
}
