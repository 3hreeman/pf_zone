using UnityEngine;

public class UnitBase : MonoBehaviour {
    private float attackCoolTime = 0;
    private float curAtkCooltime = 0;
    
    protected int maxHp = 100;
    protected int curHp = 100;

    public Vector3 dirVector { get; protected set; }
    public Vector2 aimVector { get; protected set; }

    protected virtual void DoMove() { }
    public virtual void DoAttack(Vector3 end) { }

    protected void DoRespawn() {
        transform.position = Vector3.zero;
        curHp = maxHp;
    }
    
    
    public void TakeDmg(int dmg) {
        curHp -= dmg;
        if (curHp <= 0) {
            DoRespawn();
        }
        
        var pos = transform.position;
        pos.y += 1;
        CombatDmgFontObject.PrintDmgFont(pos, dmg.ToString(), CombatDmgFontObject.DmgTxtType.NormalAtk, 0);
    }
}
