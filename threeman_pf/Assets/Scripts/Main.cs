using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour {
    [Range(0.1f, 5f)] public float enemyGenTime = 5f;
    [Range(0.1f, 0.5f)] public float enemyGenTimeDecrease = 0.1f;
    public PlayerUnit player;
    public EnemyUnit enemyPrefab;

    private IObjectPool<EnemyUnit> enemyPool;

    private void Start() {
        enemyPool = new ObjectPool<EnemyUnit>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize: 20);
        enemyGenTime = 5;
        StartCoroutine(DoEnemyGen());
    }
    
    IEnumerator DoEnemyGen() {
        while (true) {
            var enemy = enemyPool.Get();
            float x = player.transform.position.x + Random.Range(-5.0f, 5.0f);
            float y = player.transform.position.y + Random.Range(-5.0f, 5.0f);
            enemy.transform.position = new Vector3(x,y, 0);
            enemy.InitEnemy(player);
            yield return new WaitForSeconds(enemyGenTime);
        }
    }

    private void Update() {
        if (enemyGenTime > 0.1f) {
            enemyGenTime = (enemyGenTime - Time.deltaTime * enemyGenTimeDecrease);
        }
        else {
            enemyGenTime = 0.1f;
        }
    }

    private EnemyUnit CreateEnemy() {
        var enemy = Instantiate(enemyPrefab).GetComponent<EnemyUnit>();
        enemy.SetPool(enemyPool);
        return enemy;
    }
    
    private void OnGetEnemy(EnemyUnit enemy) {
        enemy.gameObject.SetActive(true);
    }
    
    private void OnReleaseEnemy(EnemyUnit enemy) {
        enemy.gameObject.SetActive(false);
    }
    
    private void OnDestroyEnemy(EnemyUnit enemy) {
        Destroy(enemy.gameObject);
    }
}
