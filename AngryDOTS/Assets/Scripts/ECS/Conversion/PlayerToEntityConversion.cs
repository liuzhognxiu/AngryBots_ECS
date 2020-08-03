using Assets.Scripts.ECS.Data;
using Unity.Entities;
using UnityEngine;

public class PlayerToEntityConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public float healthValue = 1f;


    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponent(entity, typeof(PlayerTag));

        Health health = new Health { Value = healthValue, beHitValue = 0 };
        manager.AddComponentData(entity, health);

        Damage damage = new Damage { value = 0f};
        manager.AddComponentData(entity, damage);
    }
}