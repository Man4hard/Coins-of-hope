using UnityEngine;

namespace CoinsOfHope.Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Coins of Hope/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level Info")]
        public int levelNumber = 1;
        public string levelName = "Level 1";

        [Header("Difficulty Settings")]
        [Range(1f, 10f)]
        public float scrollSpeed = 3f;

        [Range(0.5f, 5f)]
        public float obstacleSpawnInterval = 2f;

        [Range(0f, 1f)]
        public float obstacleSpawnChance = 0.5f;

        [Header("Coin Settings")]
        [Range(0.5f, 3f)]
        public float coinSpawnInterval = 1.5f;

        [Range(0f, 1f)]
        public float coinSpawnChance = 0.7f;

        [Header("Rewards")]
        public int coinsPerSecond = 5;
    }
}
