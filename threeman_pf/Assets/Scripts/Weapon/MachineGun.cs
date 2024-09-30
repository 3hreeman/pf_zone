using UnityEngine;

public class MachineGun : WeaponObject {
    public float _fireRate = 0.1f;
    
    private float _nextTimeToFire = 0f;

    public override void RotateWeapon() {
        //weapon's right side is facing the aim direction
        var aimPos = unitObj.aimVector;
        var playerPos = unitObj.transform.position;
        aimPos.x += playerPos.x;
        aimPos.y += playerPos.y;
        aimObject.right = aimPos - (Vector2)playerPos;
    }
}
