using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] _enemyPrefabs;

    [Header("Attributes")]
    [SerializeField] private int _baseEnemies = 8;
    [SerializeField] private float _enemiesPerSecond = 0.5f;
    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private float _difficultyScalingFactor = 0.75f;

    [Header("Events")]
    public static UnityEvent _onEnemyDestroy = new UnityEvent();
    private int _currentWave = 1;
    private int _enemiesAlive;
    private int _enemiesLeftToSpawn;
    private bool _isSpawning = false;
    private float _timeSinceLastSpawn;

    [Header("Enemy Factory")]
    [SerializeField] private FlyerFactory _flyerFactory;
    [SerializeField] private TankFactory _tankFactory;
    [SerializeField] private WarriorFactory _warriorFactory;

    private void Awake()
    {
        _onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!_isSpawning) return;

        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn >= (1f / _enemiesPerSecond) && _enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();

            _enemiesLeftToSpawn--;
            _enemiesAlive++;
            _timeSinceLastSpawn = 0f;
        }

        if (_enemiesAlive == 0 && _enemiesLeftToSpawn == 0)
        {
            EndWave();
        } 
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(_timeBetweenWaves);

        _isSpawning = true;
        _enemiesLeftToSpawn = EnemiesPerWave();
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, 3);
        EnemyFactory selectedFactory;

        switch (randomIndex)
        {
            case 0:
                selectedFactory = _flyerFactory;
                break;
            case 1:
                selectedFactory = _tankFactory;
                break;
            case 2:
                selectedFactory = _warriorFactory;
                break;
            default:
                selectedFactory = _flyerFactory; 
                break;
        }

        selectedFactory.Generate(LevelManager.main.startPoint.position);
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(_baseEnemies * Mathf.Pow(_currentWave, _difficultyScalingFactor));
    }

    private void EnemyDestroyed()
    {
        _enemiesAlive--;
    }

    private void EndWave()
    {
        _isSpawning = false;
        _timeSinceLastSpawn = 0f;
        _currentWave++;
        StartCoroutine(StartWave());
    }
}
