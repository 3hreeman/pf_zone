using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour {
    public static GameMain instance;
    public enum TestType {
        None,
        UseMain,
        UseUpdate,
        UseJob
    }
    [Range(0.1f, 5f)] public float enemyGenTime = 5f;
    [Range(0.1f, 0.5f)] public float enemyGenTimeDecrease = 0.1f;
    public PlayerUnit player;
    public EnemyUnit enemyPrefab;

    private IObjectPool<EnemyUnit> enemyPool;

    private CancellationTokenSource cts = new();

    [FormerlySerializedAs("enemyGenerator")] public EnemyUpdater enemyUpdater;
    
    public List<EnemyUnit> enemyList = new List<EnemyUnit>();

    public TestType testType = TestType.UseMain;

    public bool useEnemyGen = false;
    public int InitCount = 1000;
    
    private void Start() {
        instance = this;
        enemyUpdater.Init(this);
        enemyPool = new ObjectPool<EnemyUnit>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize: 2000);
        enemyGenTime = 5;
        for (int i = 0; i < InitCount; i++) {
            GenerateEnemy();
        }
        TaskEnemyGen(cts.Token).Forget();
        
        //for generator
    }
  
    private async UniTaskVoid TaskEnemyGen(CancellationToken token) {
        while (true) {
            if (useEnemyGen) {
                GenerateEnemy();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(enemyGenTime), cancellationToken: token);  //token.Cancel()이 호출되면 
        }
    }
    
    public void GenerateEnemy() {
        var pos = player.transform.position;
        var enemy = enemyPool.Get();
        float x = pos.x + Random.Range(-5.0f, 5.0f);
        float y = pos.y + Random.Range(-5.0f, 5.0f);
        enemy.transform.position = new Vector3(x,y, 0);
        enemy.InitEnemy(player);
    }
    
    private void Update() {
        if (enemyGenTime > 0.1f) {
            enemyGenTime = (enemyGenTime - Time.deltaTime * enemyGenTimeDecrease);
        }
        else {
            enemyGenTime = 0.1f;
        }
        
        if (enemyUpdater.currentEnemyCount > EnemyUpdater.MAX_ENEMY_COUNT) {
            Debug.LogWarning("Stop Enemy Generate");
            cts.Cancel();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            testType = TestType.UseMain;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            testType = TestType.UseJob;
        }
        
        if (testType == TestType.UseJob) {
            TestJob();
        }else if (testType == TestType.UseMain) {
            TestUpdate();
        }
    }

    void TestUpdate() {
        foreach(var enemy in enemyList) {
            enemy.UpdateEnemy();
        }            
    }

    void TestJob() {
        enemyUpdater.EnemyUpdate(player.transform);

    }
    
    private void LateUpdate() {
        if (testType == TestType.UseJob) {
            enemyUpdater.EnemyLateUpdate();
        }
    }

    private EnemyUnit CreateEnemy() {
        var enemy = Instantiate(enemyPrefab).GetComponent<EnemyUnit>();
        enemy.SetPool(enemyPool);
        return enemy;
    }
    
    private void OnGetEnemy(EnemyUnit enemy) {
        enemy.gameObject.SetActive(true);
        enemyList.Add(enemy);
        enemyUpdater.AddEnemy(enemy);
    }
    
    private void OnReleaseEnemy(EnemyUnit enemy) {
        enemy.gameObject.SetActive(false);
        int idx = enemyList.IndexOf(enemy);
        enemyUpdater.RemoveEnemy(idx);
        enemyList.Remove(enemy);
    }
    
    private void OnDestroyEnemy(EnemyUnit enemy) {
        Destroy(enemy.gameObject);
    }
}
