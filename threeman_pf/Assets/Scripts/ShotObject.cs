using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ShotObject : MonoBehaviour {
#region pooling code
    private IObjectPool<ShotObject> _objPool;
    
    public void SetPool(IObjectPool<ShotObject> pool) {
        _objPool = pool;
    }
    
    public void ReleaseObject() {
        _objPool.Release(this);
    }
#endregion

    [SerializeField]
    private Transform shot_transform;
    
    private Vector3 dir;
    private float moveSpd = 10;
    
    private static GameObject playerObj;
    private float lifeTime;

    private bool isAlive = false;
    private int shotDmg = 0;
    
    public void ShotStart(Vector3 start, Vector3 end, int dmg) {
        transform.position = start;
        dir = (end - start).normalized;
        dir.z = 0;
        lifeTime = 0;
        shotDmg = dmg;
        isAlive = true;
    }
    
    private void ShotEnd() {
        isAlive = false;
        gameObject.SetActive(false);
        ReleaseObject();
    }

    void Update() {
        if(isAlive == false) return;
        
        lifeTime += Time.deltaTime;
        if(lifeTime > 2f) {
            ShotEnd();
        }
        transform.Translate(dir * (Time.deltaTime * moveSpd));
        shot_transform.right = dir;
    }
    
    private void OnTriggerEnter2D(Collider2D col) {
        if (!isAlive) return;
        
        if (col.gameObject.CompareTag("EnemyUnit")) {
            var enemy = col.gameObject.GetComponent<PlayerUnit>();
            enemy.TakeDmg(shotDmg);
            ShotEnd();
        }
    }
}