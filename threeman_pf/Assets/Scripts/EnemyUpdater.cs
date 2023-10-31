using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using MRandom = Unity.Mathematics.Random;

public class EnemyUpdater : MonoBehaviour {
    public const int MAX_ENEMY_COUNT = 10000;

    public GameMain main;
    private PositionUpdateJob posUpdateJob;
    private JobHandle posUpdateJobHandle;
    
    private TransformAccessArray transformAccessArray;

    [Range(1, 5)] public float enemyMoveSpd;
    public int currentEnemyCount => transformAccessArray.length;
    public void Init(GameMain main) {
        this.main = main;
        transformAccessArray = new TransformAccessArray(MAX_ENEMY_COUNT);
    }

    public void AddEnemy(EnemyUnit unit) {
        transformAccessArray.Add(unit.transform);
    }
    
    public void RemoveEnemy(int idx) {
        //TODO need specified enemy remove logic 
        transformAccessArray.RemoveAtSwapBack(idx);
    }

    public void EnemyUpdate(Transform playerTransform) {
        posUpdateJob = new PositionUpdateJob() {
            targetPosition = playerTransform.position,
            jobDeltaTime = Time.deltaTime,
            moveSpd = enemyMoveSpd
        };
        
        posUpdateJobHandle = posUpdateJob.Schedule(transformAccessArray);
    }
    
    public void EnemyLateUpdate() {
        posUpdateJobHandle.Complete();
    }
   
    private void OnDestroy() {
        transformAccessArray.Dispose();
    }
    
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform {
        public Vector2 targetPosition;

        public float jobDeltaTime;
        public float moveSpd;

        public void Execute(int i, TransformAccess transform) {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpd * jobDeltaTime);
        }
    }
}
