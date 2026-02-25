using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    [SerializeField] private Transform _spawnPoint;

    public void Spawn()
    {
        Instantiate(_prefab, _spawnPoint.position, _spawnPoint.rotation);
    }
}
