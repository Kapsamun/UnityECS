using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MovementSystem))]
public partial struct PlayerAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        bool attackPressed = Input.GetKeyDown(KeyCode.E);
        float dt = SystemAPI.Time.DeltaTime;

        if (attackPressed)
        {
            Debug.Log("<color=cyan>[PlayerAttack]</color> Нажата клавиша E! Запуск OverlapSphere...");
        }

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

        var job = new PlayerAttackJob
        {
            CollisionWorld = collisionWorld,
            DeltaTime = dt,
            AttackPressed = attackPressed,
            EnemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false)
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
[WithAll(typeof(PlayerTag))]
[WithNone(typeof(DeadTag))]
public partial struct PlayerAttackJob : IJobEntity
{
    [ReadOnly] public CollisionWorld CollisionWorld;
    public float DeltaTime;
    public bool AttackPressed;

    [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;

    // Разрешаем параллельным потокам записывать в один компонент Health (отключаем защиту параллельного доступа)
    [NativeDisableParallelForRestriction] public ComponentLookup<Health> HealthLookup;

    public void Execute(ref AttackRadius attack, in LocalTransform transform)
    {
        if (attack.Timer > 0f)
        {
            attack.Timer -= DeltaTime;
            if (attack.Timer < 0f)
                attack.Timer = 0f;
        }

        if (!AttackPressed || attack.Timer > 0f)
            return;

        var filter = CollisionFilter.Default; 

       /* var filter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u,
            GroupIndex = 0
        };*/

        var hits = new NativeList<DistanceHit>(Allocator.TempJob);

        // Поиск коллайдеров в радиусе через CollisionWorld
        if (CollisionWorld.OverlapSphere(transform.Position, attack.Value, ref hits, filter))
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Entity hitEntity = hits[i].Entity;

                if (!EnemyLookup.HasComponent(hitEntity) || !HealthLookup.HasComponent(hitEntity))
                    continue;

                var enemyHealth = HealthLookup[hitEntity];
                enemyHealth.Current -= attack.Damage;
                HealthLookup[hitEntity] = enemyHealth;
            }
        }

        attack.Timer = attack.Cooldown;
        hits.Dispose();
    }
}