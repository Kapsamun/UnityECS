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

public struct TargetPosition : IComponentData
{
    public float3 Value;
}


public struct PrefabsHolder : IComponentData
{
    public Entity EnemyPrefab;
}

public enum EnemyType : byte
{
    Basic = 0,
    Fast = 1
}

public struct WavesBlob
{
    public BlobArray<Wave> Waves;
}

/// <summary>
/// Одна волна: пауза перед стартом + список групп врагов.
/// </summary>
public struct Wave
{
    public float WaitTimeBeforeWave;
    public BlobArray<EnemyInWave> Enemies;
}

/// <summary>
/// Сколько врагов какого типа спавнить в волне.
/// </summary>
public struct EnemyInWave
{
    public EnemyType EnemyType;
    public int Amount;
}

/// <summary>
/// Компонент-держатель ссылки на Blob
/// </summary>
public struct WavesHolder : IComponentData
{
    public BlobAssetReference<WavesBlob> WavesReference;
}

/// <summary>
/// Runtime-состояние контроллера волн (какой номер волны, таймер).
/// </summary>
public struct WaveState : IComponentData
{
    public int CurrentWaveIndex;
    public float Timer;
    public bool IsWaiting;      // ждём WaitTimeBeforeWave
    public bool AllWavesDone;
    public float SpawnRadius;   // радиус спавна вокруг игрока
}

/// <summary>
/// Метка: entity должна быть уничтожена.
/// HealthSystem ставит тег, DeathSystem уничтожает через ECB.
/// </summary>
public struct DeadTag : IComponentData { }
