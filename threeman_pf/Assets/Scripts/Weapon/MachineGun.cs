using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : WeaponObject {
    public float _fireRate = 0.1f;
    public float _damage = 10f;
    public float _range = 100f;
    public float _force = 30f;
    public float _spread = 0.1f;
    
    private float _nextTimeToFire = 0f;

    public override void DoFire(UnitBase unit, Vector3 end, float chargingPower = 1) {
        if (Time.time >= _nextTimeToFire) {
            _nextTimeToFire = Time.time + 1f / _fireRate;
        }
    }
}
