using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DirectorEnemyManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float minSpawnDistance = 10f;  
    public float maxSpawnDistance = 40f;  
    public int maxClusters = 2;
    public int maxEnemiesPerCluster = 3;
    public float clusterEnemySpacing = 3f;
    
    [Header("Enemy Spawning Timings")]
    private float spawnTimer;
    private bool wasEmpty = false;  
    public float spawnIntervalMin = 40f;  
    public float spawnIntervalMax = 60f;  
    public float alterSpawnTime = 0f;
    
    [Header("Max Enemies Allowed:")]
    public int maxEnemies = 10;        

    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs;
    private string[] spawnableEnemies = new string[] {};


    [Header("Alter Event Settings")]
    public float alterSpawnIntervalMin = 10f;
    public float alterSpawnIntervalMax = 20f;
    public int alterMinSpawn = 2;
    public int alterMaxSpawn = 5;
    public bool alterEvent = false;
    private GameObject[] spawnPoints;
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    // Managers 
    private Transform playerTransform;
    DirectorStageManager stageManager;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found. Set the player tag.");
        }

        // Set spawn timer, and wasEmpty to false so the first spawn is quicker
        wasEmpty = false;
        spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);

        stageManager = GameObject.FindObjectOfType<DirectorStageManager>();
        LoadEnemyPrefabs();
    }

    void Update()
    {
        // Clear dead enemies
        activeEnemies.RemoveAll(enemy => enemy == null);

        // Alter event
        if (alterEvent)
        {
            spawnAlterEnemies();
        }

        // Quicker spawn timer for when no enemies are left
        if (activeEnemies.Count == 0 && !wasEmpty)
        {
            spawnTimer = 10f;
            wasEmpty = true;
        }
        else if (activeEnemies.Count > 0)
        {
            wasEmpty = false;
        }

        if (activeEnemies.Count < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                SpawnEnemyCluster();
                spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }
    }

    void spawnAlterEnemies()
    {
        maxEnemies = 20;
        
        if (alterSpawnTime <= 0)
        {
            // Spawn random number of enemies in random locations around the alter
            int numEnemies = Random.Range(alterMinSpawn, alterMaxSpawn + 1);
            alterSpawnTime = Random.Range(alterSpawnIntervalMin, alterSpawnIntervalMax);

            for (int i = 0; i <= numEnemies; i++)
            {
                // Randomly select an enemy prefab and spawn point
                string enemyName = spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
                GameObject enemyPrefab = enemyPrefabs.Find(prefab => prefab.name == enemyName);
                Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                activeEnemies.Add(enemy);
            }
        }
        else
        {
            alterSpawnTime -= Time.deltaTime;
        }
    }

    void LoadEnemyPrefabs()
    {
        // Setting the spawnable enemies for different stage types
        if (stageManager.sceneName == "PlainsStage")
        {
            spawnableEnemies = new string[] {
                "RockGolem",
                "Spider",
                "Skeleton"
            };
        }
        else if (stageManager.sceneName == "SnowyStage")
        {
            spawnableEnemies = new string[] {
                "IceRockGolem",
                "Spider",
                "Skeleton"
            };
        }
        else
        {
            spawnableEnemies = new string[] {
                "RockGolem",
                "IceRockGolem",
                "Spider",
                "Skeleton"
            };
        }
    }

    void SpawnEnemyCluster()
    {
        if (playerTransform == null || enemyPrefabs.Count == 0)
            return;

        // Random number of enemy clusters to spawn
        int numClusters = Random.Range(1, maxClusters + 1);

        for (int i = 0; i < numClusters; i++)
        {
            // Get a valid spawn position
            Vector3 clusterCenter = GetValidSpawnPosition();
            if (clusterCenter == Vector3.zero) continue; 

            // Random number of enemies to spawn in the cluster
            int numEnemies = Random.Range(1, maxEnemiesPerCluster + 1);
            
            // Randomly select an enemy prefab
            string enemyName = spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
            GameObject enemyPrefab = enemyPrefabs.Find(prefab => prefab.name == enemyName);
        
            if (enemyPrefab == null)
            {
                Debug.LogError($"Could not find prefab for enemy: {enemyName}");
                continue;
            }

            GameObject firstEnemy = Instantiate(enemyPrefab, clusterCenter, Quaternion.identity);
            activeEnemies.Add(firstEnemy);

            if (numEnemies > 1)
            {
                // Spawn enemies in a triangle formation around the first enemy
                Vector3 directionToPlayer = (playerTransform.position - clusterCenter).normalized;
                
                for (int j = 1; j < numEnemies; j++)
                {
                    Vector3 offset = GetTriangleOffset(j, directionToPlayer);
                    Vector3 spawnPos = clusterCenter + offset;

                    if (IsValidSpawnPosition(spawnPos))
                    {
                        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                        activeEnemies.Add(enemy);
                    }
                }
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 20; 
        for (int i = 0; i < maxAttempts; i++)
        {
            // Randomly select a spawn position around the player
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 randomPos = playerTransform.position + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance
            );

            if (IsValidSpawnPosition(randomPos))
            {
                // Sample the navmesh to make sure enemy is within the map and can move
                UnityEngine.AI.NavMeshHit navHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
        }
        return Vector3.zero; 
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // Raycast down to check if the position is on the ground
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out hit, 200f))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                // Sample the navmesh to make sure enemy is within the map and can move
                UnityEngine.AI.NavMeshHit navHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(hit.point, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    return true;
                }
            }
        }
        return false;
    }

    Vector3 GetTriangleOffset(int index, Vector3 directionToPlayer)
    {
        Vector3 perpendicular = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
        
        switch (index)
        {
            case 1:
                return perpendicular * clusterEnemySpacing;
            case 2: 
                return -perpendicular * clusterEnemySpacing;
            default:
                return Vector3.zero;
        }
    }

    void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnDistance);
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnDistance);
        }
    }
}