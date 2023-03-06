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
    
    private Action endCallback = null;

    private static GameObject playerObj;
    private float lifeTime;
    public void ShotStart(Vector3 start, Vector3 end, Action callback = null) {
        transform.position = start;
        dir = (end - start).normalized;
        dir.z = 0;
        endCallback = callback;
        lifeTime = 0;
    }
    
    private void ShotEnd() {
        if (endCallback != null) {
            endCallback();
        }
        gameObject.SetActive(false);
        ReleaseObject();
    }

    void Update() {
        lifeTime += Time.deltaTime;
        if(lifeTime > 2f) {
            ShotEnd();
        }
        transform.Translate(dir * (Time.deltaTime * moveSpd));
        shot_transform.right = dir;
    }
}