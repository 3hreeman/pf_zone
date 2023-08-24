using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingObject : MonoBehaviour {
#region pooling base
    protected IObjectPool<PoolingObject> _objPool;
    protected Transform _poolParent;    //release된 object들을 모아두는 parent
    public void SetPool(IObjectPool<PoolingObject> pool, Transform poolParent) {
        _objPool = pool;
        _poolParent = poolParent;
    }

    public virtual void OnTakeObject(PoolingObject pObj) {
        pObj.gameObject.SetActive(true);
    }

    public virtual void OnReleaseObject(PoolingObject pObj) {
        pObj.transform.SetParent(_poolParent);
        pObj.gameObject.SetActive(false);
    }

    public virtual void OnDestroyObject(PoolingObject pObj) {
        Destroy(pObj);
    }

    public virtual void ReleaseObject() {
        _objPool.Release(this);
    }

#endregion
}
