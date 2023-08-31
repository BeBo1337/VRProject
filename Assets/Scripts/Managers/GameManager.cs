using System.Collections;
using System.Collections.Generic;
using DataTypes;
using Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private List<Material> _materialsByHP;
        [SerializeField] private List<EnemySpawnPoint> _spawnPoints;
        [SerializeField] private float _minimumDelayBetweenSpanws;
        [SerializeField] private float _maxDelayBetweenSpanws;
        // TODO: difficulty multiplier
        
        private float _timeToNextSpawn;
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;
        
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
            StartGame();
        }

        private void StartGame()
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
            AudioManager.Instance.PlayMusic(SoundType.BattleMusic);
            ResetSession();
            _isPlaying = true;
            SpawnNewEnemy();
            SetNextSpawnTime();
        }

        private void ResetSession()
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
            var spawnPoint = _spawnPoints.GetRandom();
            newEnemy.Initialize(spawnPoint.transform.position, GetRandomEnemyHealth());
            newEnemy.UpdateMoving(true);
            AudioManager.Instance.PlaySoundEffect(spawnPoint.AudioSource, SoundType.EnemySpawn);
        }

        private int GetRandomEnemyHealth()
        {
            return Random.Range(1, _materialsByHP.Count + 1);
        }

        public Material GetEnemyMaterial(int remainingHealth)
        {
            var index = remainingHealth - 1;
            var materialShouldExist = index > -1 && index < _materialsByHP.Count;
            if (materialShouldExist)
            {
                return _materialsByHP[index];   
            }
            else
            {
                return null;
            }
        }
        
        public void BulletHitEnemy(Bullet bullet, Enemy enemy)
        {
            BulletSpawner.BaseInstance.Release(bullet);
            var shouldDestroy = enemy.ReduceHealth();
            if (shouldDestroy)
            {
                enemy.Explode();
                EnemySpawner.BaseInstance.Release(enemy);
                
                // increase score + hud
                _score++;
                PlayerHUDManager.Instance.SetScore(_score);
            }
        }

        private void SetNextSpawnTime()
        {
            _timeToNextSpawn = Random.Range(_minimumDelayBetweenSpanws, _maxDelayBetweenSpanws);
        }
    }
}