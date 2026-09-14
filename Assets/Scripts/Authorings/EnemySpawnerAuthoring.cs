using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;     // обычный GameObject-префаб врага (с EnemyAuthoring)
    public float SpawnInterval = 3f;   // каждые n секунд
    public int SpawnCount = 15;        // m врагов
    public float SpawnRadius = 25f;    // на каком расстоянии от игрока
}

public class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        // TransformUsageFlags.None — спавнеру не нужен Transform
        var entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new EnemySpawner
        {
            Prefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
            Interval = authoring.SpawnInterval,
            Count = authoring.SpawnCount,
            Radius = authoring.SpawnRadius,
            Timer = 0f
        });
    }
}