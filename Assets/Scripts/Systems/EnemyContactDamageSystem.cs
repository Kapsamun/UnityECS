using Unity.Burst;
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
    [Unity.Collections.ReadOnly] public ComponentLookup<PlayerTag> PlayerLookup;
    [Unity.Collections.ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
    [Unity.Collections.ReadOnly] public ComponentLookup<ContactDamage> ContactDamageLookup;
    public ComponentLookup<Health> HealthLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        TryDamagePlayer(triggerEvent.EntityA, triggerEvent.EntityB);
        TryDamagePlayer(triggerEvent.EntityB, triggerEvent.EntityA);
    }

    void TryDamagePlayer(Entity maybePlayer, Entity maybeEnemy)
    {
        if (!PlayerLookup.HasComponent(maybePlayer)) return;
        if (!EnemyLookup.HasComponent(maybeEnemy)) return;
        if (!ContactDamageLookup.HasComponent(maybeEnemy)) return;
        if (!HealthLookup.HasComponent(maybePlayer)) return;

        float damage = ContactDamageLookup[maybeEnemy].Value;
        RefRW<Health> playerHealth = HealthLookup.GetRefRW(maybePlayer);
        playerHealth.ValueRW.Current -= damage * 0.016f;
    }
}