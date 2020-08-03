using Assets.Scripts.ECS.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveForwardSystem))]
[UpdateBefore(typeof(TimedDestroySystem))]
public class CollisionSystem : JobComponentSystem
{
    EntityQuery enemyGroup;
    EntityQuery bulletGroup;
    EntityQuery playerGroup;

    protected override void OnCreate()
    {
        playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<Damage>());
        enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<Damage>());
        bulletGroup = GetEntityQuery(typeof(TimeToLive), ComponentType.ReadOnly<Translation>());
    }

    // [BurstCompile]
    struct CollisionJob : IJobChunk
    {
        public float radius;

        public ArchetypeChunkComponentType<Health> healthType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
        [ReadOnly] public ArchetypeChunkComponentType<Damage> damageType;

        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> transToTestAgainst;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkHealths = chunk.GetNativeArray(healthType);
            var chunkTranslations = chunk.GetNativeArray(translationType);
            var chunkDamages = chunk.GetNativeArray(damageType);

            for (int i = 0; i < chunk.Count; i++)
            {
                // float damage = 0f;
                Health health = chunkHealths[i];
                Translation pos = chunkTranslations[i];
                Damage damage = chunkDamages[i];

                for (int j = 0; j < transToTestAgainst.Length; j++)
                {
                    Translation pos2 = transToTestAgainst[j];

                    if (CheckCollision(pos.Value, pos2.Value, radius))
                    {
                        health.Value -= damage.value;
                        health.beHitValue = damage.value;
                        chunkHealths[i] = health;
                    }
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var healthType = GetArchetypeChunkComponentType<Health>(false);
        var translationType = GetArchetypeChunkComponentType<Translation>(true);
        var damageType = GetArchetypeChunkComponentType<Damage>(true);

        float enemyRadius = Settings.EnemyCollisionRadius;
        float playerRadius = Settings.PlayerCollisionRadius;

        var jobEvB = new CollisionJob()
        {
            radius = enemyRadius * enemyRadius,
            healthType = healthType,
            translationType = translationType,
            damageType = damageType,
            transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
        };

        JobHandle jobHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

        if (Settings.IsPlayerDead())
            return jobHandle;

        var jobPvE = new CollisionJob()
        {
            radius = playerRadius * playerRadius,
            healthType = healthType,
            translationType = translationType,
            damageType = damageType,
            transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
        };

        return jobPvE.Schedule(playerGroup, jobHandle);
    }

    static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.z * delta.z;

        return distanceSquare <= radiusSqr;
    }
}
