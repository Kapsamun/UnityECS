using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerInputSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 input = float2.zero;

        if (Input.GetKey(KeyCode.W))
            input.y += 1;
        if (Input.GetKey(KeyCode.S))
            input.y -= 1;
        if (Input.GetKey(KeyCode.A))
            input.x -= 1;
        if (Input.GetKey(KeyCode.D))
            input.x += 1;

        if (math.lengthsq(input) > 0)
            input = math.normalize(input);

        foreach (var (velocity, speed) in
                 SystemAPI.Query<RefRW<Velocity>, RefRO<MoveSpeed>>()
                          .WithAll<PlayerTag>())
        {
            velocity.ValueRW.Value = new float3(input.x, 0, input.y) * speed.ValueRO.Value;
        }
    }
}