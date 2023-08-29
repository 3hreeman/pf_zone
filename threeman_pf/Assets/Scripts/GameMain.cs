using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour {
    [Range(0.1f, 5f)] public float enemyGenTime = 5f;
    [Range(0.1f, 0.5f)] public float enemyGenTimeDecrease = 0.1f;
    public PlayerUnit player;
    public EnemyUnit enemyPrefab;

    private IObjectPool<EnemyUnit> enemyPool;

    private CancellationTokenSource cts = new();
    
    private void Start() {
        enemyPool = new ObjectPool<EnemyUnit>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize: 20);
        enemyGenTime = 5;
        StartCoroutine(DoEnemyGen());
    }

    private async UniTaskVoid TestTask1(CancellationToken token) {
        //token.Cancel이 불릴 경우 TestTask1이 중단된다
        
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        Debug.LogWarning("1 sec passed");
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        Debug.LogWarning("2 sec passed");
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        Debug.LogWarning("3 sec passed");
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        Debug.LogWarning("4 sec passed");
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        Debug.LogWarning("5 sec passed");
    }

    private async UniTaskVoid TaskEnemyGen(CancellationToken token) {
        while (true) {
            var pos = player.transform.position;
            var enemy = enemyPool.Get();
            float x = pos.x + Random.Range(-5.0f, 5.0f);
            float y = pos.y + Random.Range(-5.0f, 5.0f);
            enemy.transform.position = new Vector3(x,y, 0);
            enemy.InitEnemy(player);
            await UniTask.Delay(TimeSpan.FromSeconds(enemyGenTime), cancellationToken: token);  //token.Cancel()이 호출되면 
        }
    }
    
    IEnumerator DoEnemyGen() {
        while (true) {
            var pos = player.transform.position;
            var enemy = enemyPool.Get();
            float x = pos.x + Random.Range(-5.0f, 5.0f);
            float y = pos.y + Random.Range(-5.0f, 5.0f);
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

        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.LogWarning("Stop Enemy Generate");
            cts.Cancel();
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
