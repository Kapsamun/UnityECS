// Assets/Scripts/Systems/DebugDamageSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
public partial struct DebugDamageSystem : ISystem
{
    private float _lastPlayerHealth;

    public void OnCreate(ref SystemState state)
    {
        _lastPlayerHealth = 100f;
    }

    public void OnUpdate(ref SystemState state)
    {
        // ќтслеживаем изменение здоровь€ игрока (контактный урон от физики)
        foreach (var health in SystemAPI.Query<RefRO<Health>>().WithAll<PlayerTag>())
        {
            float current = health.ValueRO.Current;
            if (current < _lastPlayerHealth - 0.5f) // если сн€ли больше 0.5 HP
            {
                Debug.Log($"<color=red>[ онтактный урон]</color> »грок атакован врагом! “екущее HP: {current:F1}");
                _lastPlayerHealth = current;
            }
        }

        // Ћ ћ Ч отладочный урон всем врагам
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

        // K Ч ручное отн€тие 25 HP
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>().WithAll<PlayerTag>())
            {
                health.ValueRW.Current = math.max(0, health.ValueRO.Current - 25f);
                Debug.Log($"Player Health: {health.ValueRO.Current}");
            }
        }

        // H Ч проверить текущее здоровье игрока
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>().WithAll<PlayerTag>())
            {
                Debug.Log($"Player Health: {health.ValueRO.Current}");
            }
        }
    }
}