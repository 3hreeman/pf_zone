using UnityEngine;

public class UnitBase : MonoBehaviour {
    public enum UnitType {
        Player,
        Enemy
    }
    protected float nextAttackTime = 0;
    
    protected int maxHp = 10;
    protected int curHp = 10;

    public bool isAlive = false;
    public UnitType unitType;
    public Vector3 dirVector { get; protected set; }
    public Vector2 aimVector { get; protected set; }

    protected virtual void DoMove() { }
    public virtual void DoAttack(Vector3 end) { }

    protected virtual void DoDie() {
        isAlive = false;
    }

    public virtual void TakeDmg(int dmg) {
        if(isAlive == false) return;
        
        curHp -= dmg;
        if (curHp <= 0) {
            DoDie();
        }
        
        var pos = transform.position;
        pos.y += 1;
        CombatDmgFontObject.PrintDmgFont(pos, dmg.ToString(), CombatDmgFontObject.DmgTxtType.NormalAtk, 0);
    }
}
