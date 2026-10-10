using Unity.Entities;
using UnityEngine;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
public partial struct DebugDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>()
                                            .WithAll<EnemyTag>()
                                            .WithNone<DeadTag>())
            {
                health.ValueRW.Current -= 15f;
            }
            Debug.Log("Damaged all enemies by 15");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>()
                                            .WithAll<PlayerTag>())
            {
                health.ValueRW.Current -= 25f;
                Debug.Log($"Player Health: {health.ValueRO.Current}");
            }
        }
    }
}
