using UnityEngine;
using UnityEngine.Pool;

public class WeaponObject : MonoBehaviour {
    [SerializeField]
    private ShotObject shotObject;
    private IObjectPool<ShotObject> _objPool;

    public void Init() {
        _objPool = new ObjectPool<ShotObject>(CreateShot, OnGetShot, OnReleaseShot, OnDestroyShot, maxSize:20);
    }

    public void DoFire(Vector3 start, Vector3 end) {
        var shot = _objPool.Get();
        shot.ShotStart(start, end);
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
