using Unity.Entities;
using Unity.Mathematics;

public struct Health : IComponentData
{
    public float Current;
    public float Max;
}

public struct MoveSpeed : IComponentData
{
    public float Value;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}

public struct PlayerTag : IComponentData { }

public struct EnemyTag : IComponentData { }

// Для врагов — цель (позиция игрока). Можно хранить Entity игрока, но для простоты float3.
public struct TargetPosition : IComponentData
{
    public float3 Value;
}

public struct EnemySpawner : IComponentData
{
    public Entity Prefab;          // Entity-префаб врага
    public float Interval;         // n секунд
    public int Count;              // m врагов за раз
    public float Radius;           // радиус спавна вокруг центра
    public float Timer;            // внутренний таймер
}