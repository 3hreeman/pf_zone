using System;
using UnityEngine;

public class ShotObject : PoolingObject {
    [SerializeField]
    private Transform shot_transform;
    
    private Vector3 dir;
    private float moveSpd = 10;
    
    private UnitBase owner;
    private static GameObject playerObj;
    private float lifeTime;

    private bool isAlive = false;
    private float baseDmg = 0;
    private float chargingPower = 0;
    private int shotDmg = 0;
    private int shotLife = 0;
    private int curLife = 0;
    private UnitBase.UnitType ownerType;

    public SpriteRenderer sprRenderer;
    public void SetShotSprite(Sprite spr) {
        sprRenderer.sprite = spr;
    }
    
    public void ShotStart(UnitBase unit, Vector3 start, Vector3 end, float shotSpeed, float baseDmg, float chargingPower) {
        transform.position = start;
        dir = (end - start).normalized;
        dir.z = 0;
        lifeTime = 0;
        moveSpd = shotSpeed;
        this.baseDmg = baseDmg;
        this.chargingPower = chargingPower;
        shotDmg = Mathf.FloorToInt(baseDmg * chargingPower);
        curLife = shotLife = Mathf.FloorToInt(chargingPower);
        transform.localScale = Vector3.one * chargingPower;
        owner = unit;
        ownerType = unit.unitType;
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
    
    /*private void OnTriggerStay2D(Collider2D col) {
        if (!isAlive) return;
        
        if (col.gameObject.CompareTag("Obstacle")) {
            var hitFx = ObjectPoolManager.instance.Get<SimpleHitFxObject>("fx_hit");
            hitFx.transform.position = transform.position;
            hitFx.SetDirection(owner.transform.position - transform.position);
            ShotEnd();
        }
        if(curLife > 0) {
            if (ownerType == UnitBase.UnitType.Player) {
                if (col.gameObject.CompareTag("EnemyUnit")) {
                    var enemy = col.gameObject.GetComponent<EnemyUnit>();
                    enemy.TakeDmg(shotDmg);
                    var hitFx = ObjectPoolManager.instance.Get<SimpleHitFxObject>("fx_hit");
                    hitFx.transform.position = transform.position;
                    hitFx.SetDirection(owner.transform.position - transform.position);
                    curLife--;
                } 
            }
            else if (ownerType == UnitBase.UnitType.Enemy) {
                if (col.gameObject.CompareTag("PlayerUnit")) {
                    var player = col.gameObject.GetComponent<PlayerUnit>();
                    player.TakeDmg(shotDmg);
                    curLife--;
                }
            }
            
            if(curLife <= 0) {
                ShotEnd();
            }
        }
    }*/

    private void OnCollisionEnter2D(Collision2D col) {
        if (!isAlive) return;
        
        if (col.gameObject.CompareTag("Obstacle")) {
            var hitFx = ObjectPoolManager.instance.Get<SimpleHitFxObject>("fx_hit");
            hitFx.transform.position = transform.position;
            hitFx.SetDirection(owner.transform.position - transform.position);
            ShotEnd();
        }
        
        if(curLife > 0) {
            if (ownerType == UnitBase.UnitType.Player) {
                if (col.gameObject.CompareTag("EnemyUnit")) {
                    var enemy = col.gameObject.GetComponent<EnemyUnit>();
                    enemy.TakeDmg(shotDmg);
                    var hitFx = ObjectPoolManager.instance.Get<SimpleHitFxObject>("fx_hit");
                    hitFx.transform.position = transform.position;
                    hitFx.SetDirection(owner.transform.position - transform.position);
                    curLife--;
                }
            }
            else if (ownerType == UnitBase.UnitType.Enemy) {
                if (col.gameObject.CompareTag("PlayerUnit")) {
                    var player = col.gameObject.GetComponent<PlayerUnit>();
                    player.TakeDmg(shotDmg);
                    curLife--;
                }
            }
            
            if(curLife <= 0) {
                ShotEnd();
            }
        }
    }
}