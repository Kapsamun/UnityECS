using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;    
    public float SpawnInterval = 3f;   
    public int SpawnCount = 15;        
    public float SpawnRadius = 25f;  
}

public class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
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