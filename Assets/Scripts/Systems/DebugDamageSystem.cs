using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
public partial struct DebugDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>()
                                            .WithAll<EnemyTag>()
                                            .WithNone<DeadTag>())
            {
                health.ValueRW.Current = math.max(0, health.ValueRO.Current - 15f);
            }
            Debug.Log("Damaged all enemies by 15");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>()
                                            .WithAll<PlayerTag>())
            {
                health.ValueRW.Current = math.max(0, health.ValueRO.Current - 25f);
                Debug.Log($"Player Health: {health.ValueRO.Current}");
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>().WithAll<PlayerTag>())
            {
                Debug.Log($"Player Health: {health.ValueRO.Current}");
            }
        }
    }
}