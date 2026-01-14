using System.Collections;
using CoinsOfHope;
using CoinsOfHope.Audio;
using CoinsOfHope.Data;
using CoinsOfHope.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoinsOfHope.Gameplay
{
    public class CoinRunnerManager : MonoBehaviour
    {
        public static CoinRunnerManager Instance { get; private set; }

        [Header("Level Config")]
        public LevelConfig levelConfig;

        [Header("Player")]
        public PlayerController player;

        [Header("Spawner Settings")]
        public Transform spawnPoint;
        public float despawnX = -10f;

        private bool _gameActive;
        private float _elapsedTime;
        private int _coinsCollected;

        private float _obstacleSpawnTimer;
        private float _coinSpawnTimer;

        private bool _completionTriggered;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager not found! Returning to MainMenu.");
                SceneManager.LoadScene(SceneNames.MainMenu);
                return;
            }

            if (levelConfig == null)
            {
                var allConfigs = Resources.LoadAll<LevelConfig>("");
                if (allConfigs != null && allConfigs.Length > 0)
                {
                    int selectedLevel = GameManager.Instance.SelectedLevel;
                    foreach (var cfg in allConfigs)
                    {
                        if (cfg.levelNumber == selectedLevel)
                        {
                            levelConfig = cfg;
                            break;
                        }
                    }

                    if (levelConfig == null) levelConfig = allConfigs[0];
                }
                else
                {
                    Debug.LogWarning("No LevelConfig found in Resources. Creating default.");
                    levelConfig = CreateDefaultLevelConfig();
                }
            }

            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController>();
            }

            if (spawnPoint == null)
            {
                var sp = new GameObject("SpawnPoint");
                sp.transform.position = new Vector3(10f, 0, 0);
                spawnPoint = sp.transform;
            }

            StartGame();

            if (AudioManager.Instance != null && AudioManager.Instance.gameplayMusic != null)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
            }
        }

        void Update()
        {
            if (!_gameActive) return;

            _elapsedTime += Time.deltaTime;

            if (player != null && !player.IsAlive)
            {
                EndGame();
                return;
            }

            _obstacleSpawnTimer -= Time.deltaTime;
            if (_obstacleSpawnTimer <= 0)
            {
                _obstacleSpawnTimer = levelConfig.obstacleSpawnInterval;
                if (Random.value < levelConfig.obstacleSpawnChance)
                {
                    SpawnObstacle();
                }
            }

            _coinSpawnTimer -= Time.deltaTime;
            if (_coinSpawnTimer <= 0)
            {
                _coinSpawnTimer = levelConfig.coinSpawnInterval;
                if (Random.value < levelConfig.coinSpawnChance)
                {
                    SpawnCoin();
                }
            }

            CleanupOffscreenObjects();
        }

        void StartGame()
        {
            _gameActive = true;
            _elapsedTime = 0;
            _coinsCollected = 0;
            _completionTriggered = false;

            _obstacleSpawnTimer = levelConfig.obstacleSpawnInterval;
            _coinSpawnTimer = levelConfig.coinSpawnInterval;
        }

        void EndGame()
        {
            if (!_gameActive) return;
            _gameActive = false;

            if (_completionTriggered) return;
            _completionTriggered = true;

            int earnedCoins = Mathf.RoundToInt(_elapsedTime * levelConfig.coinsPerSecond) + _coinsCollected;
            earnedCoins = Mathf.Max(0, earnedCoins);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel(levelConfig.levelNumber, earnedCoins);
            }

            StartCoroutine(ReturnToMenuAfterDelay(2f));
        }

        IEnumerator ReturnToMenuAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        void SpawnObstacle()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Obstacle";
            go.tag = "Obstacle";
            go.transform.position = spawnPoint.position;
            go.transform.localScale = new Vector3(1f, 1f, 1f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;

            var mover = go.AddComponent<ObstacleMover>();
            mover.speed = levelConfig.scrollSpeed;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
        }

        void SpawnCoin()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Coin";
            go.tag = "Coin";

            float yOffset = Random.Range(-2f, 3f);
            go.transform.position = spawnPoint.position + new Vector3(0, yOffset, 0);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;

            var mover = go.AddComponent<ObstacleMover>();
            mover.speed = levelConfig.scrollSpeed;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
        }

        void CleanupOffscreenObjects()
        {
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (var obj in obstacles)
            {
                if (obj.transform.position.x < despawnX)
                {
                    Destroy(obj);
                }
            }

            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            foreach (var coin in coins)
            {
                if (coin.transform.position.x < despawnX)
                {
                    Destroy(coin);
                }
            }
        }

        public void OnCoinCollected()
        {
            _coinsCollected++;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoinCollect();
            }
        }

        public void OnPlayerHitObstacle()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayObstacleHit();
            }
            EndGame();
        }

        public void PauseGame()
        {
            _gameActive = false;
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            _gameActive = true;
            Time.timeScale = 1;
        }

        void OnDestroy()
        {
            Time.timeScale = 1;
        }

        LevelConfig CreateDefaultLevelConfig()
        {
            var cfg = ScriptableObject.CreateInstance<LevelConfig>();
            cfg.levelNumber = 1;
            cfg.levelName = "Level 1";
            cfg.scrollSpeed = 5f;
            cfg.obstacleSpawnInterval = 2f;
            cfg.obstacleSpawnChance = 0.5f;
            cfg.coinSpawnInterval = 1.5f;
            cfg.coinSpawnChance = 0.7f;
            cfg.coinsPerSecond = 5;
            return cfg;
        }
    }
}
