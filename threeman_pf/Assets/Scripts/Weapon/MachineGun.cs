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
}
