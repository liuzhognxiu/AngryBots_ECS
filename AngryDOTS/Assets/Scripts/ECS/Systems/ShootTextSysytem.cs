using Assets.Scripts.ECS.Data;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ShootTextSysytem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Health health, ref Translation pos,ref Damage damage) =>
        {
            if (health.beHitValue > 0)
            {
                ColliderPool.PlayBulletImpact(pos.Value,health.beHitValue,entity.Index);
                health.beHitValue = 0f;
            }
        });
    }
}