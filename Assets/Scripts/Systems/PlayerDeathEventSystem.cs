using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
public partial struct PlayerDeathEventSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithAll<PlayerTag>().WithEntityAccess())
        {
            if (health.ValueRO.Current <= 0f)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ShowGameOver();
                }

                // Отключаем систему, чтобы событие сработало ровно один раз
                state.Enabled = false;
                break;
            }
        }
    }
}