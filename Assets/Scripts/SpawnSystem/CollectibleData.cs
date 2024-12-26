using UnityEngine;

// CollectibleData is a ScriptableObject that stores data for collectibles
[CreateAssetMenu(fileName = "CollectibleData", menuName = "Collectible Data")]
public class CollectibleData : EntityData
{
    public int score;
    // additional properties specific to collectibles
}
