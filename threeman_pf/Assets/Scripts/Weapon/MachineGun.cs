using UnityEngine;

public class MachineGun : WeaponObject {
    public override void RotateWeapon() {
        //weapon's right side is facing the aim direction
        var aimPos = unitObj.aimVector;
        var playerPos = unitObj.transform.position;
        aimPos.x += playerPos.x;
        aimPos.y += playerPos.y;
        aimObject.right = aimPos - (Vector2)playerPos;
    }
    
    public override void DoFire(UnitBase unit, Vector3 end, float chargingPower = 1f) {
        var shot = ObjectPoolManager.instance.Get("shot_object") as ShotObject;
        shot.SetShotSprite(shotSpr);
        shot.ShotStart(unit, fireStartTransform.position, end, baseShotSpeed, baseDamage, chargingPower);
        PlayFireFx();
    }
}
