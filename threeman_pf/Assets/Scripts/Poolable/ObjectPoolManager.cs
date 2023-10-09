using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ObjectPoolManager : MonoBehaviour {
    [Serializable]
    public class PoolInfo {
        public string key;
        public PoolingObject poolObj;
        public int defaultCapacity;
        public int maxPoolSize;     
    }
    public List<PoolInfo> poolInfoList;         //등록된 object들에 대해서만 풀링함
    public static ObjectPoolManager instance;
    
    private Dictionary<string, PoolInfo> infoDict;
    public Dictionary<string, IObjectPool<PoolingObject>> poolDict;
    public Transform poolParent;
    
    private string curKey;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        infoDict = poolInfoList.ToDictionary(e => e.key, e => e);
        poolDict = new Dictionary<string, IObjectPool<PoolingObject>>();
        foreach (var info in poolInfoList) {
            InitPool(info);
        }
    }

    private void InitPool(PoolInfo poolInfo) {
        var poolObj = poolInfo.poolObj;
        curKey = poolInfo.key;
        Debug.LogFormat("InitPool - {0} ", curKey);
        var newPool = new ObjectPool<PoolingObject>(OnCreateObject, poolObj.OnTakeObject, poolObj.OnReleaseObject, poolObj.OnDestroyObject,
            true, poolInfo.defaultCapacity, poolInfo.maxPoolSize);
        poolDict.Add(poolInfo.key, newPool);
    }

    private PoolingObject OnCreateObject() {
        var poolObj = Instantiate(infoDict[curKey].poolObj).GetComponent<PoolingObject>();
        poolObj.SetPool(poolDict[curKey], poolParent);
        return poolObj;
    }
    
    public PoolingObject Get(string key) {
        curKey = key;
        
        if (instance.poolDict.TryGetValue(curKey, out var resultPool)) {
            var result = resultPool.Get();
            return result;
        }

        Debug.LogWarningFormat("ObjectPoolManager::Get {0} is not registered key", curKey);
        return null;
    }

    /*private void Update() {
        TestInputUpdate();
    }*/

    private void TestInputUpdate() {
        if (Input.GetMouseButton(0)) {
            /*var poolObj = Get("fx_bomb");
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            poolObj.transform.position = pos;*/
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            CombatDmgFontObject.PrintDmgFont(pos, Random.Range(10000,100000).ToString(), CombatDmgFontObject.DmgTxtType.NormalAtk, 0);
        }else if (Input.GetMouseButton(1)) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            var poolObj = Get("dmg_font") as DmgFontObject;
            poolObj.PrintDmgFont(pos, Random.Range(10000,100000).ToString(), DmgFontObject.DmgTxtType.Crit);
        }
    }
}