using UnityEngine;

public class UnitBase : MonoBehaviour {
    private float attackCoolTime = 0;
    private float curAtkCooltime = 0;
    
    protected int maxHp = 100;
    protected int curHp = 100;

    public bool isAlive = false;

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
