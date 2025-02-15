using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class EnemyRespawner : MonoBehaviour
{
    // [SerializeField] private EnemyInstantiator _enemyInstantiator;
    [SerializeField] private List<Transform> _respawnPoints;
    [SerializeField] private Vector3Variable _playerPosition;
    [SerializeField] private float _timeBetweenRespawn;
    [SerializeField] private int _pointsOfRespawnToUse;
    [SerializeField] private GameObject _prefabGetterContainer;
    
    private IGameObject _prefabGetter;

    public UnityEvent OnRespawn;

    private void Awake() => _prefabGetter = _prefabGetterContainer.GetComponent<IGameObject>();

    public void RespawnEnemies(int enemiesAmount)
    {
        // Debug.Log($"Enemies to respawn: {enemiesAmount}");
        Vector3[] respawnPoints = GetNClosestPoints(_pointsOfRespawnToUse,_playerPosition.Value);
        // Debug.Log($"RespawnPoints: {respawnPoints.Length}");
        int enemiesPerRespawn = enemiesAmount / respawnPoints.Length;
        // Debug.Log($"Enemies per respawn: {enemiesPerRespawn}");
        int restOfEnemies = enemiesAmount % respawnPoints.Length;
        // Debug.Log($"RestOfEnemies: {restOfEnemies}");
        foreach (var point in respawnPoints)
        {
            for (var i = 0; i < enemiesPerRespawn; i++)
            {
                this.Invoke(() => {
                    OnRespawn?.Invoke();
                    CreateEnemy(point,_prefabGetter.GameObject);
                },_timeBetweenRespawn * i);
            }
        }

        if(restOfEnemies > 0)
        {
            this.Invoke(() =>
            {
                for (var i = 0; i < restOfEnemies; i++)
                {
                    OnRespawn?.Invoke();
                    CreateEnemy(respawnPoints[i],_prefabGetter.GameObject);
                }
            },_timeBetweenRespawn * enemiesPerRespawn);
        }
    }

    Vector3[] GetNClosestPoints(int n, Vector3 playerPosition)
    {
        List<Vector3> closestPoints = new List<Vector3>();

        for (int i = 0; i < _respawnPoints.Count; i++)
        {
            if(closestPoints.Count < n)
            {
                closestPoints.Add(_respawnPoints[i].position);
            }
            else
            {
                for (int j = 0; j < closestPoints.Count; j++)
                {
                    float savedPointDistance = Vector3.Distance(closestPoints[j], playerPosition);
                    float checkingPointDistance = Vector3.Distance(_respawnPoints[i].position,playerPosition);
                    if(checkingPointDistance < savedPointDistance)
                    { 
                        closestPoints.Remove(closestPoints[j]);
                        closestPoints.Add(_respawnPoints[i].position);
                    }
                }
            }
        }
        return closestPoints.ToArray();
    }

    public void AddRespawnPoint(GameObject respawnPoint)
    {
        _respawnPoints.Add(respawnPoint.transform);
    }

    GameObject CreateEnemy(Vector3 position, GameObject prefab)
    {
        return Instantiate
        (
            prefab,
            position,
            Quaternion.identity
        );
    }
}