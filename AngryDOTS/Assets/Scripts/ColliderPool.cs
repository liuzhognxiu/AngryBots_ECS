using UnityEngine;

public class ColliderPool : MonoBehaviour
{
    static ColliderPool instance;

    public static int damagePoolSize = 100;
    ShootTextProController[] damagePool;
    int currentPoolIndex;

    public ShootTextProController shootTextProController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;

        damagePool = new ShootTextProController[damagePoolSize];
        for (int i = 0; i < damagePoolSize; i++)
        {
            damagePool[i] = Instantiate(shootTextProController, instance.transform);
        }
    }

    public static void PlayBulletImpact(Vector3 position, float damage, int index)
    {
        if (index == 0)
        {
            return;
        }
        if (index >= instance.damagePool.Length)
            index %= damagePoolSize;

        instance.damagePool[index].transform.position = position;
        instance.damagePool[index].DelayMoveTime = 0.4f;
        instance.damagePool[index].textAnimationType = TextAnimationType.Burst;
        instance.damagePool[index].CreatShootText("-" + damage, instance.damagePool[index].transform);
    }
}
