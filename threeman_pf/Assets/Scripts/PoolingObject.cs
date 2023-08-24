using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingObject : MonoBehaviour {
    #region pooling base
    private IObjectPool<PoolingObject> _objPool;

    private bool usePooling = false;
    public void SetPool(IObjectPool<PoolingObject> pool) {
        _objPool = pool;
        usePooling = true;
    }

    public virtual void OnTakeObject(PoolingObject pObj) {
        pObj.releaseAt = Time.time + 2;
        pObj.gameObject.SetActive(true);
    }

    public virtual void OnReleaseObject(PoolingObject pObj) {
        pObj.gameObject.SetActive(false);
    }

    public virtual void OnDestroyObject(PoolingObject pObj) {
        Destroy(pObj.gameObject);
    }
    #endregion

    public float releaseAt = 0;

    public void Start() {
        releaseAt = Time.time + 2;
    }
    
    public void Update() {
        if (Time.time > releaseAt) {
            if (usePooling) {
                _objPool.Release(this);
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}
