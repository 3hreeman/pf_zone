using System;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponObject : MonoBehaviour {
    [SerializeField] private ShotObject shotObject;
    [SerializeField] private Transform aimObject;
    
    private IObjectPool<ShotObject> _objPool;

    private bool isInit = false;
    private UnitBase unitObj;
    public Vector2 aimPos;
    
    public void Init(UnitBase player) {
        unitObj = player;
        _objPool = new ObjectPool<ShotObject>(CreateShot, OnGetShot, OnReleaseShot, OnDestroyShot, maxSize:20);
        isInit = true;
    }

    public void Update() {
        if(isInit == false) return;
        aimPos = unitObj.aimVector;
        var playerPos = unitObj.transform.position;
        aimPos.x += playerPos.x;
        aimPos.y += playerPos.y;
        aimObject.position = aimPos;
    }
    
    public void DoFire(Vector3 end) {
        var shot = _objPool.Get();
        shot.ShotStart(aimPos, end, 1);
    }

    private ShotObject CreateShot() {
        var shot = Instantiate(shotObject).GetComponent<ShotObject>();
        shot.SetPool(_objPool);
        return shot;
    }

    private void OnGetShot(ShotObject shot) {
        shot.gameObject.SetActive(true);
    }
    
    private void OnReleaseShot(ShotObject shot) {
        shot.gameObject.SetActive(false);
    }
    
    private void OnDestroyShot(ShotObject shot) {
        Destroy(shot.gameObject);
    }
}
