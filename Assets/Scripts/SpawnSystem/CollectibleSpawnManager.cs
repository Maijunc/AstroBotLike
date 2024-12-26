using UnityEngine;
using static Timer;
public class CollectibleSpawnManager : EntitySpawnManager 
{
    // CollectibleData包含了Collectible的Prefab和其他属性
    [SerializeField] CollectibleData[] collectibleData;
    [SerializeField] float spawnInterval = 1f;

    // EntitySpawner是一个泛型类，用于生成Collectible，里面包含了Collectible的工厂和生成点策略
    EntitySpawner<Collectible> spawner;

    CountdownTimer spawnTimer;
    int counter;

    protected override void Awake()
    {
        base.Awake();

        // EntitySpawner 需要一个工厂和一个生成点策略 只需要是实现了IEntityFactory<Collectible>和ISpawnPointStrategy的类即可 具体细节怎么实现可以随意调整
        spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(collectibleData), spawnPointStrategy);
    
        spawnTimer = new CountdownTimer(spawnInterval);
        spawnTimer.OnTimerStop += () => {
            // 在生成点线性生成
            if (counter++ >= spawnPoints.Length) {
                spawnTimer.Stop();
                return;
            }
            Spawn();
            spawnTimer.Start();
        };
    }

    void Start() => spawnTimer.Start();

    void Update() => spawnTimer.Tick(Time.deltaTime);

    public override void Spawn() => spawner.Spawn();
}