using UnityEngine;

public class EntityFactory<T> : IEntityFactory<T> where T : Entity 
{
    EntityData[] data;

    public EntityFactory(EntityData[] data)
    {
        this.data = data;
    }

    public T Create(Transform spawnPoint)
    {
        // 这里使用随机表示可以随机生成不同的实体
        EntityData entityData = data[Random.Range(0, data.Length)];
        GameObject instance = GameObject.Instantiate(entityData.prefab, spawnPoint.position, spawnPoint.rotation);
        return instance.GetComponent<T>();
    }
}
