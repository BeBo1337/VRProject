using System.Collections;
using System.Collections.Generic;
using DataTypes;
using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private List<EnemySpawnPoint> _spawnPointsLVL1;
        [SerializeField] private List<EnemySpawnPoint> _spawnPointsLVL2;
        [SerializeField] private List<EnemySpawnPoint> _spawnPointsLVL3;

        private float _timeToNextSpawn;
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        public static bool _gameOver = false;
        public static int _level = 1;
        private int _score;
        private float _timer;
        private GameObject _gun;
        public static GameManager Instance { get; private set; }
        
        private const string gunString = "Gun";
        private const string mainMenuString = "MainMenu";    
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // This method is called when the game starts.
        private void Start()
        {
            _gun = GameObject.FindWithTag(gunString); 
            StartLevel();
        }

        // Initiates the game level.
        private void StartLevel()
        {
            StartCoroutine(PlayerHUDManager.Instance.PlayCountdownAnimation()); // Display a countdown animation.
            StartCoroutine(WaitForAnimation()); // Wait for the countdown animation to finish.
        }

        // Waits for the countdown animation to finish before starting the actual game.
        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(PlayerHUDManager.Instance.AnimationDuration);
            StartActualGame(); // Start the actual game after the countdown.
        }

        // Starts the actual game, plays background music, and spawns the first enemy.
        private void StartActualGame()
        {
            AudioManager.Instance.PlayMusic(SoundType.BattleMusic); // Play background battle music.
            _isPlaying = true; 
            SpawnNewEnemy(); 
            SetNextSpawnTime(); 
        }

        // Sets the game state to "Game Over" and perform cleanup actions.
        public void SetGameOver()
        {
            _gameOver = true;
            _isPlaying = false; // Stop spawning enemies.
            ReleaseAllEnemies(); // Destroy all remaining enemies.
            _gun.gameObject.SetActive(false);
            PlayerHUDManager.Instance.GameOver(_score, _timer); // Display the game over UI.
            
            if(_score < 30) // Lost -> play sound
                AudioManager.Instance.PlaySoundEffect(SoundType.GameOver); // Play game over sound.

            ResetGame();
            Invoke("LoadMainMenu", 8f);
        }

        private void ResetGame()
        {
            _gameOver = false;
            _level = 1;
            _score = 0;
            _timer = 0;
        }

        // Loads the main menu scene.
        private void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuString);
        }
        
        // This method is called once per frame while the game is running.
        public void Update()
        {
            if (_isPlaying)
            {
                _timeToNextSpawn -= Time.deltaTime; // Countdown for the next enemy spawn.
                if (_timeToNextSpawn < 0)
                {
                    SpawnNewEnemy(); // Spawn a new enemy when the timer reaches zero.
                    SetNextSpawnTime(); 
                }

                _timer += Time.deltaTime; 
                PlayerHUDManager.Instance.SetTime(_timer); // Update the displayed game timer on the HUD.
            }
        }

        // Spawns a new enemy at a random spawn point.
        private void SpawnNewEnemy()
        {
            var newEnemy = EnemySpawner.BaseInstance.Spawn(); 
            var spawnPoint = GetSpawner(); // Get a random spawn point.

            // Reset the position of the new enemy to the chosen spawn point.
            Vector3 spawnPosition = spawnPoint.transform.position;
            newEnemy.transform.position = spawnPosition;

            // Initialize the enemy with random health and speed.
            newEnemy.Initialize(spawnPoint.transform.position, GetEnemyHealth(), GetEnemySpeed());
            AudioManager.Instance.PlaySoundEffect(spawnPoint.AudioSource, SoundType.EnemySpawn); // Play spawn sound effect.
        }

        // Sets a random time delay for the next enemy spawn.
        private void SetNextSpawnTime()
        {
            switch (_level)
            {
                case 1:
                    _timeToNextSpawn = Random.Range(3f, 5f);
                    break;
                case 2:
                    _timeToNextSpawn = Random.Range(3f, 4.5f);
                    break;
                case 3:
                    _timeToNextSpawn = Random.Range(2.5f, 3.5f);
                    break;
                default:
                    _timeToNextSpawn = Random.Range(3f, 5f);
                    break;
            }
            
        }

        // Determine the speed of the spawned enemy based on the current game level.
        private float GetEnemySpeed()
        {
            switch (_level)
            {
                case 1:
                    return Random.Range(0.5f, 0.75f);
                case 2:
                    return Random.Range(0.65f, 1f);
                case 3:
                    return Random.Range(0.7f, 1.1f);
                default:
                    return Random.Range(0.5f, 0.75f);
            }
        }

        // Determine the health of the spawned enemy based on the current game level.
        private int GetEnemyHealth()
        {
            switch (_level)
            {
                case 1:
                    return Random.Range(3, 4);
                case 2:
                    return Random.Range(3, 5);
                case 3:
                    return Random.Range(4, 5);
                default:
                    return 4;
            }
        }

        // Choose a random spawn point based on the current game level.
        private EnemySpawnPoint GetSpawner()
        {
            switch (_level)
            {
                case 1:
                    return _spawnPointsLVL1.GetRandom();
                case 2:
                    return _spawnPointsLVL2.GetRandom();
                case 3:
                    return _spawnPointsLVL3.GetRandom();
                default:
                    return _spawnPointsLVL1.GetRandom();
            }
        }

        // Handle bullet hit events on enemies.
        public void BulletHitEnemy(Enemy enemy, HitType bodyPartHit)
        {
            if (!enemy.isDead)
            {
                var shouldDestroy = enemy.ReduceHealth(bodyPartHit); // Reduce enemy health.
                if (shouldDestroy)
                {
                    // Increase the player's score and update the HUD.
                    _score++;
                    PlayerHUDManager.Instance.SetScore(_score);
                    enemy.Die(); // Destroy the enemy.

                    if ((_score >= 5 && _level == 1) || (_score >= 15 && _level == 2))
                    {
                        // If the score reaches the checkpoint goal, trigger level progression.
                        StartCoroutine(PlayerHUDManager.Instance.NextLevelAnimation());
                        PathFollowing.isMoving = true; 
                        _isPlaying = false; // Stop spawning enemies.
                        ReleaseAllEnemies(); // Release all remaining enemies.
                        StartCoroutine(WaitForPathCompletion()); // Wait for path following to complete.
                    }
                    else if (_score >= 30 && _level == 3)
                    {
                        // Trigger gameover not from death = win
                        SetGameOver();
                    }
                }
                else
                {
                    enemy.Hit();
                }
            }
        }

        // Wait for the player path following to the next level to complete before starting the level.
        private IEnumerator WaitForPathCompletion()
        {
            yield return new WaitUntil(() => !PathFollowing.isMoving);

            // After the path is completed, restart the level.
            _level++;
            StartLevel();
        }

        // Releases all enemies in the scene.
        private void ReleaseAllEnemies()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                if (_gameOver)
                    enemy.gameObject.SetActive(false); 
                enemy.Die(); 
            }
        }
    }
}