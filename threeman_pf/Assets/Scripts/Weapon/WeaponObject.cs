using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponObject : MonoBehaviour {
    public AudioSource audioSource;
    
    public AudioClip fireSound;
    
    [SerializeField] protected Transform aimObject;
    [SerializeField] protected Sprite shotSpr;
    protected bool isInit = false;
    protected UnitBase unitObj;
    public Vector2 aimPos;
    public Transform fireStartTransform;
    
    protected float baseCooltime = 0.25f;
    protected float baseShotSpeed = 10f;
    
    public List<ParticleSystem> fireFxList = new List<ParticleSystem>();
    
    public void Init(UnitBase player) {
        unitObj = player;
        isInit = true;
    }

    public virtual void UpdateWeapon() {
        if(isInit == false) return;
        RotateWeapon();
    }

    public virtual float GetFireCooltime() {
        return baseCooltime;
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
        shot.ShotStart(unit, fireStartTransform.position, end, baseShotSpeed, chargingPower);
        audioSource.PlayOneShot(fireSound);
        foreach (var fx in fireFxList) {
            fx.Play();
        }
    }
}