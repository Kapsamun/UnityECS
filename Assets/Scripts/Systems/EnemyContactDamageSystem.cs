
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
public partial struct EnemyContactDamageSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        var job = new EnemyContactDamageJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            PlayerLookup = SystemAPI.GetComponentLookup<PlayerTag>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            ContactDamageLookup = SystemAPI.GetComponentLookup<ContactDamage>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false)
        };

        state.Dependency = job.Schedule(simulation, state.Dependency);
    }
}

[BurstCompile]
public partial struct EnemyContactDamageJob : ITriggerEventsJob
{
    public float DeltaTime;

    [ReadOnly] public ComponentLookup<PlayerTag> PlayerLookup;
    [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
    [ReadOnly] public ComponentLookup<ContactDamage> ContactDamageLookup;
    public ComponentLookup<Health> HealthLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        TryApplyContactDamage(triggerEvent.EntityA, triggerEvent.EntityB);
        TryApplyContactDamage(triggerEvent.EntityB, triggerEvent.EntityA);
    }

    private void TryApplyContactDamage(Entity maybePlayer, Entity maybeEnemy)
    {
        if (PlayerLookup.HasComponent(maybePlayer) && EnemyLookup.HasComponent(maybeEnemy))
        {
            if (ContactDamageLookup.HasComponent(maybeEnemy) && HealthLookup.HasComponent(maybePlayer))
            {
                float damage = ContactDamageLookup[maybeEnemy].Value;

                var health = HealthLookup[maybePlayer];
                health.Current -= damage * DeltaTime;
                HealthLookup[maybePlayer] = health;
            }
        }
    }
}