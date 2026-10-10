using Unity.Entities;
using UnityEngine;

/// <summary>
/// Authoring для Singleton-хранилища префабов.
/// </summary>
public class PrefabsHolderAuthoring : MonoBehaviour
{
    [Tooltip("Префаб врага (GameObject с EnemyAuthoring)")]
    public GameObject EnemyPrefab;
}

public class PrefabsHolderAuthoringBaker : Baker<PrefabsHolderAuthoring>
{
    public override void Bake(PrefabsHolderAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new PrefabsHolder
        {
            EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic)
        });
    }
}
