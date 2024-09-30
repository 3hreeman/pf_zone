using UnityEngine;

public class WeaponObject : MonoBehaviour {
    [SerializeField] private Transform aimObject;
    [SerializeField] private Sprite shotSpr;
    private bool isInit = false;
    private UnitBase unitObj;
    public Vector2 aimPos;
    
    public void Init(UnitBase player) {
        unitObj = player;
        isInit = true;
    }

    public virtual void UpdateWeapon() {
        if(isInit == false) return;
        RotateWeapon();
    }

    public virtual void RotateWeapon() {
        aimPos = unitObj.aimVector;
        var playerPos = unitObj.transform.position;
        aimPos.x += playerPos.x;
        aimPos.y += playerPos.y;
        aimObject.position = aimPos;
    }
    
    public virtual void DoFire(UnitBase unit, Vector3 end, float chargingPower = 1f) {
        var shot = ObjectPoolManager.instance.Get("shot_object") as ShotObject;
        shot.SetShotSprite(shotSpr);
        shot.ShotStart(unit, aimPos, end, chargingPower);
    }
}
