using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase {
    private const float MOVE_SPD = 5f;
    private const float DASH_TIME = 0.5f;
    private const float DASH_COOLTIME = 1f;
    [Range(1f, 10f)] public float AtkSpdRatio = 1f;
    [Range(1f, 10f)] public float AtkPowerRatio = 1f;
    private float m_leftDashTime = 0;
    private float m_nextDashTime = 0;

    [SerializeField] private List<WeaponObject> weaponList;
    
    [SerializeField] private WeaponObject currentWeapon;
    [SerializeField] private CharacterView charView;

    private bool isCharging = false;
    private float chargingPower = 1;
    private float maxChargingPower = 5f;
    private void Start() {
        InitWeapon();
        ChangeWeapon(0);
        currentWeapon.Init(this);
        curHp = maxHp;
        isAlive = true;
    }

    private void InitWeapon() {
        foreach(var weapon in weaponList) {
            weapon.Init(this);
        }
    }

    public void Update() {
        DoMove();
        currentWeapon.UpdateWeapon();
    }
    
    public void ChangeNextWeapon() {
        var idx = weaponList.IndexOf(currentWeapon);
        idx++;
        if (idx >= weaponList.Count) {
            idx = 0;
        }
        ChangeWeapon(idx);
    }
    
    public void ChangeWeapon(int idx) {
        if (idx < 0 || idx >= weaponList.Count) return;
        if(currentWeapon != null) {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeapon = weaponList[idx];
        currentWeapon.gameObject.SetActive(true);
    }

    private bool CheckDashAvailable() {
        return m_nextDashTime < Time.time;
    }

    public void UpdateAim(Vector3 mousePos) {
        aimVector = (mousePos - transform.position).normalized;
    }

    public void UpdateDir(Vector3 dir) {
        dirVector = dir;
    }

    public void SetCharging(bool flag) {
        isCharging = flag;
        chargingPower = 1f;
    }

    public void UpdateCharging() {
        if (isCharging) {
            chargingPower += Time.deltaTime;
            if (chargingPower > maxChargingPower) {
                chargingPower = maxChargingPower;
            }
        }
    }
    
    public void DoRolling() {
        if (!CheckDashAvailable()) {
            return;
        }
        m_leftDashTime = Time.time + DASH_TIME;
        m_nextDashTime = Time.time + DASH_COOLTIME;
        charView.DoRolling(dirVector, DASH_TIME);
    }
    
    public override void DoAttack(Vector3 end) {
        if (isCharging) {
            currentWeapon.DoFire(this, end, chargingPower * 3f * AtkPowerRatio);
            SetCharging(false);
            return;
        } 
        if (nextAttackTime > Time.time) return;
        
        nextAttackTime = Time.time + (currentWeapon.GetFireCooltime() / AtkSpdRatio);
        currentWeapon.DoFire(this, end, AtkPowerRatio);
    }

    protected override void DoMove() {
        if (dirVector == Vector3.zero) return;

        var moveSpd = MOVE_SPD;
        if (m_leftDashTime > Time.time) {
            moveSpd *= 3;
        }
        
        transform.Translate(dirVector * (moveSpd * Time.deltaTime));
    }
}
