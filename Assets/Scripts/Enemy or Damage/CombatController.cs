using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController Instance;
    public int maxEngagedEnemies = 2;
    public List<EnemyController> engagedEnemies = new List<EnemyController>();

    void Awake()
    {
        Debug.Log($"[CombatController] Awake called on {gameObject.name} with instance {Instance?.gameObject.name ?? "null"}");
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[CombatController] Multiple instances detected! Destroying {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log($"[CombatController] Instance set to {gameObject.name}");
    }

    void Start()
    {
        Debug.Log($"[CombatController] Start called on {gameObject.name}");
        engagedEnemies.Clear();
    }

    public void DisengageAll()
    {
        Debug.Log($"[CombatController] DisengageAll called on {gameObject.name}. Current engaged enemies: {engagedEnemies.Count}");
        engagedEnemies.Clear();
    }

    public bool TryEngage(EnemyController enemy)
    {
        Debug.Log($"[CombatController] TryEngage called on {gameObject.name} for enemy {enemy.gameObject.name}. Current engaged: {engagedEnemies.Count}");
        // Check if enemy engaged and if max number of engaged enemies is reached
        if (!engagedEnemies.Contains(enemy) && engagedEnemies.Count < maxEngagedEnemies)
        {
            engagedEnemies.Add(enemy);
            Debug.Log($"[CombatController] Enemy {enemy.gameObject.name} engaged. Total engaged: {engagedEnemies.Count}");
            return true;
        }
        Debug.Log($"[CombatController] Enemy {enemy.gameObject.name} failed to engage. Already engaged: {engagedEnemies.Contains(enemy)}, Max reached: {engagedEnemies.Count >= maxEngagedEnemies}");
        return false;
    }

    public void Disengage(EnemyController enemy)
    {
        Debug.Log($"[CombatController] Disengage called on {gameObject.name} for enemy {enemy.gameObject.name}");
        engagedEnemies.Remove(enemy);
    }
}
