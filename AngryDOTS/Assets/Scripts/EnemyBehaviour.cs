﻿using Assets.Scripts.ECS.Data;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Life Settings")]
    public float enemyHealth = 50f;

    Rigidbody rigidBody;

    ShootTextProController shootTextProController;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        shootTextProController = GetComponent<ShootTextProController>();
    }

    void Update()
    {
        if (!Settings.IsPlayerDead())
        {
            Vector3 heading = Settings.PlayerPosition - transform.position;
            heading.y = 0f;
            transform.rotation = Quaternion.LookRotation(heading);
        }

        Vector3 movement = transform.forward * speed * Time.deltaTime;
        rigidBody.MovePosition(transform.position + movement);
    }

    //Enemy Collision
    // void OnTriggerEnter(Collider theCollider)
    // {
    //     if (!theCollider.CompareTag("Bullet"))
    //         return;
    //
    //     enemyHealth--;
    //     shootTextProController.CreatShootText("-1", this.transform);
    //     if (enemyHealth <= 0)
    //     {
    //         Destroy(gameObject);
    //         BulletImpactPool.PlayBulletImpact(transform.position);
    //     }
    // }

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponent(entity, typeof(EnemyTag));
        manager.AddComponent(entity, typeof(MoveForward));

        MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
        manager.AddComponentData(entity, moveSpeed);

        Health health = new Health { Value = enemyHealth, beHitValue = 0f };
        manager.AddComponentData(entity, health);

        Damage damage = new Damage { value = 1f };
        manager.AddComponentData(entity, damage);
    }
}
