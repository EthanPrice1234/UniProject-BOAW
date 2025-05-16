using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ResetAttack : MonoBehaviour
{
    EnemyController enemyController;

    void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void ResetAttacking()
    {
        enemyController.ResetAttacking();
    }
}
