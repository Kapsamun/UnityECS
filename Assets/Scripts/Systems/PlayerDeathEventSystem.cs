using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))] // Проверяем до того, как HealthSystem уничтожит сущность игрока
public partial struct PlayerDeathEventSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // Система активна, пока жив синглтон игрока
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Ищем сущность игрока
        foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithAll<PlayerTag>().WithEntityAccess())
        {
            // Если здоровье игрока упало до нуля или ниже
            if (health.ValueRO.Current <= 0f)
            {
                // Вызываем метод в управляемом мире GameObject через синглтон GameManager
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