using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;

    [Header("Attributes")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _hp;
    [SerializeField] private float _movingSpeed;

    private Transform target;
    private int pathIndex = 0;

    private void Start()
    {
        target = LevelManager.main.path[0];
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1)
        {
            pathIndex = pathIndex + 1;

            if (pathIndex >= LevelManager.main.path.Length)
            {
                EnemySpawner._onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        _rb.velocity = direction * _movingSpeed;
    }

    public void Generate(Vector3 position)
    {
        Instantiate(_prefab, position, Quaternion.identity);
    }
}
