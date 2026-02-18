using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _prefab;

    [Header("Spawn Point (base transform)")]
    [SerializeField] private Transform _spawnPoint;

    [Header("Interval (Accelerate)")]
    [SerializeField] private float _startIntervalSeconds = 1.0f; // 開始時の間隔
    [SerializeField] private float _minIntervalSeconds = 0.2f;   // 最短間隔（下限）
    [SerializeField] private float _intervalDecreasePerSpawn = 0.02f; // 1回スポーンするごとに減らす量

    [Header("Limits")]
    [SerializeField] private int _spawnCountLimit = -1; // -1 = 無限
    [SerializeField] private int _maxAlive = -1;        // -1 = 無制限

    [Header("Random Spawn Area (Local)")]
    [SerializeField] private bool _useRandomPosition = true;
    [SerializeField] private Vector2 _areaMin = new(-5f, -5f);
    [SerializeField] private Vector2 _areaMax = new(5f, 5f);

    public enum PlaneMode { XZ, XY }
    [SerializeField] private PlaneMode _planeMode = PlaneMode.XZ;

    private readonly List<GameObject> _alive = new();
    private Coroutine _loop;

    private void OnEnable()
    {
        if (_loop == null) _loop = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (_loop != null)
        {
            StopCoroutine(_loop);
            _loop = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        var spawned = 0;
        var currentInterval = Mathf.Max(_startIntervalSeconds, _minIntervalSeconds);

        while (_spawnCountLimit < 0 || spawned < _spawnCountLimit)
        {
            yield return new WaitForSeconds(currentInterval);

            CleanupAlive();

            if (_maxAlive >= 0 && _alive.Count >= _maxAlive) continue;

            var go = SpawnOnce();
            if (go != null)
            {
                _alive.Add(go);
                spawned++;

                // だんだん早くする（下限まで）
                currentInterval = Mathf.Max(_minIntervalSeconds, currentInterval - _intervalDecreasePerSpawn);
            }
        }
    }

    private GameObject SpawnOnce()
    {
        if (_prefab == null) return null;

        var basePoint = _spawnPoint != null ? _spawnPoint : transform;
        var position = _useRandomPosition ? GetRandomPosition(basePoint) : basePoint.position;

        return Instantiate(_prefab, position, basePoint.rotation);
    }

    private Vector3 GetRandomPosition(Transform basePoint)
    {
        var minX = Mathf.Min(_areaMin.x, _areaMax.x);
        var maxX = Mathf.Max(_areaMin.x, _areaMax.x);
        var minY = Mathf.Min(_areaMin.y, _areaMax.y);
        var maxY = Mathf.Max(_areaMin.y, _areaMax.y);

        var u = Random.Range(minX, maxX);
        var v = Random.Range(minY, maxY);

        var localOffset = _planeMode == PlaneMode.XZ
            ? new Vector3(u, 0f, v)
            : new Vector3(u, v, 0f);

        return basePoint.TransformPoint(localOffset);
    }

    private void CleanupAlive()
    {
        for (var i = _alive.Count - 1; i >= 0; i--)
        {
            if (_alive[i] == null) _alive.RemoveAt(i);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var basePoint = _spawnPoint != null ? _spawnPoint : transform;

        var minX = Mathf.Min(_areaMin.x, _areaMax.x);
        var maxX = Mathf.Max(_areaMin.x, _areaMax.x);
        var minY = Mathf.Min(_areaMin.y, _areaMax.y);
        var maxY = Mathf.Max(_areaMin.y, _areaMax.y);

        Vector3[] cornersLocal =
        {
            _planeMode == PlaneMode.XZ ? new Vector3(minX, 0f, minY) : new Vector3(minX, minY, 0f),
            _planeMode == PlaneMode.XZ ? new Vector3(maxX, 0f, minY) : new Vector3(maxX, minY, 0f),
            _planeMode == PlaneMode.XZ ? new Vector3(maxX, 0f, maxY) : new Vector3(maxX, maxY, 0f),
            _planeMode == PlaneMode.XZ ? new Vector3(minX, 0f, maxY) : new Vector3(minX, maxY, 0f),
        };

        var w0 = basePoint.TransformPoint(cornersLocal[0]);
        var w1 = basePoint.TransformPoint(cornersLocal[1]);
        var w2 = basePoint.TransformPoint(cornersLocal[2]);
        var w3 = basePoint.TransformPoint(cornersLocal[3]);

        Gizmos.DrawLine(w0, w1);
        Gizmos.DrawLine(w1, w2);
        Gizmos.DrawLine(w2, w3);
        Gizmos.DrawLine(w3, w0);
    }
#endif
}
