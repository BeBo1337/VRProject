using System.Collections;
using System.Collections.Generic;
using DataTypes;
using Framework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private List<EnemySpawnPoint> _spawnPointsLVL1;
        [SerializeField] private List<EnemySpawnPoint> _spawnPointsLVL2;
        [SerializeField] private float _minimumDelayBetweenSpanws;
        [SerializeField] private float _maxDelayBetweenSpanws;
        // TODO: difficulty multiplier
        
        private float _timeToNextSpawn;
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        public static bool _gameOver = false;
        public static int _level = 1;
        private int _score;
        private float _timer;
        
        public static GameManager Instance { get; private set; }
        public string EnemyTag => "Enemy"; 

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

        private void Start()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            StartCoroutine(PlayerHUDManager.Instance.PlayCountdownAnimation());
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(PlayerHUDManager.Instance.AnimationDuration);
            StartActualGame();
        }

        private void StartActualGame()
        {
            //AudioManager.Instance.PlayMusic(SoundType.BattleMusic);
            _isPlaying = true;
            SpawnNewEnemy();
            SetNextSpawnTime();
        }

        private void OnGameOver()
        {
            _score = 0;
            _timer = 0;
        }

        public void Update()
        {
            if (_isPlaying)
            {
                _timeToNextSpawn -= Time.deltaTime;
                if (_timeToNextSpawn < 0)
                {
                    SpawnNewEnemy();
                    SetNextSpawnTime();
                }

                _timer += Time.deltaTime;
                PlayerHUDManager.Instance.SetTime(_timer);
            }
        }
        
        private void SpawnNewEnemy()
        {
            var newEnemy = EnemySpawner.BaseInstance.Spawn();
            var spawnPoint = getSpawner();
            
            //resets the position of the zombie from the pool
            Vector3 spawnPosition = spawnPoint.transform.position;
            newEnemy.transform.position = spawnPosition;
            
            newEnemy.Initialize(spawnPoint.transform.position, 4, getEnemySpeed());

            AudioManager.Instance.PlaySoundEffect(spawnPoint.AudioSource, SoundType.EnemySpawn);
        }

        private float getEnemySpeed()
        {
            switch (_level)
            {
                case 1: return Random.Range(0.5f, 0.75f);
                case 2: return Random.Range(0.65f, 1f);
                default: return Random.Range(0.5f, 0.75f);
            }
        }

        private EnemySpawnPoint getSpawner()
        {
            switch (_level)
            {
                case 1:
                    return _spawnPointsLVL1.GetRandom();
                case 2:
                    return _spawnPointsLVL2.GetRandom();
                default: 
                    return _spawnPointsLVL1.GetRandom();
            }
        }
        
        public void BulletHitEnemy(Bullet bullet, Enemy enemy, HitType bodyPartHit)
        {
            BulletSpawner.BaseInstance.Release(bullet);
            if (!enemy.isDead)
            {
                var shouldDestroy = enemy.ReduceHealth(bodyPartHit);
                if (shouldDestroy)
                {
                    // increase score + hud + go to next level
                    _score++;
                    PlayerHUDManager.Instance.SetScore(_score);
                    enemy.Die();
                    if (_score >= 2 && _level == 1)
                    {
                        PathFollowing.isMoving = true;
                        _isPlaying = false;
                        _maxDelayBetweenSpanws = 4;
                        ReleaseAllEnemies();
                        StartCoroutine(WaitForPathCompletion());
                    }
                }
                else
                {
                    enemy.Hit();
                }
            }
        }
        
        private IEnumerator WaitForPathCompletion()
        {
            yield return new WaitUntil(() => !PathFollowing.isMoving);
    
            // After the path is completed, restart the level
            StartLevel();
            _level++;
        }
        
        private void ReleaseAllEnemies()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>(); // Find all Enemy objects in the scene
            foreach (Enemy enemy in enemies)
            {
                enemy.Die();
            }
        }

        private void SetNextSpawnTime()
        {
            _timeToNextSpawn = Random.Range(_minimumDelayBetweenSpanws, _maxDelayBetweenSpanws);
        }
    }
}