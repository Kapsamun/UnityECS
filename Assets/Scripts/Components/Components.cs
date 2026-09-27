using Unity.Entities;
using Unity.Mathematics;

public struct Health : IComponentData
{
    public float Current;
    public float Max;
}

public struct Stamina : IComponentData
{
    public float Current;
    public float Max;
    public float RegenRate;   // восстановление в секунду (в покое)
    public float DrainRate;   // расход в секунду (при движении)
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

public struct TargetPosition : IComponentData
{
    public float3 Value;
}

public struct EnemySpawner : IComponentData
{
    public Entity Prefab;
    public float Interval;
    public int Count;
    public float Radius;
    public float Timer;
}