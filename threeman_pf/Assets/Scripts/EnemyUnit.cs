using UnityEngine;
using UnityEngine.Pool;

public class EnemyUnit : UnitBase {
    
#region pooling code
    private IObjectPool<EnemyUnit> _objPool;
   
    public void SetPool(IObjectPool<EnemyUnit> pool) {
        _objPool = pool;
    }
   
    public void ReleaseObject() {
        _objPool.Release(this);
    }
#endregion

    private const float MOVE_SPD = 1f;
    private const float DASH_TIME = 0.5f;
    private const float DASH_COOLTIME = 1f;

    private float m_leftDashTime = 0;
    private float m_nextDashTime = 0;

    [SerializeField] private WeaponObject weapon;
    [SerializeField] private CharacterView charView;

    private PlayerUnit playerUnit;
    
    private void Start() {
        weapon.Init(this);
    }

    public void Update() {
        DoMove();
    }

    public void InitEnemy(PlayerUnit player) {
        playerUnit = player;
        curHp = maxHp;
        SetScale(1);
        isAlive = true;
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
    
    public void DoRolling() {
        if (!CheckDashAvailable()) {
            return;
        }
        m_leftDashTime = Time.time + DASH_TIME;
        m_nextDashTime = Time.time + DASH_COOLTIME;
        charView.DoRolling(dirVector, DASH_TIME);
    }
    
    public override void DoAttack(Vector3 end) {
        weapon.DoFire(end);
    }
    
    protected override void DoDie() {
        base.DoDie();
        ReleaseObject();
    }
    
    public override void TakeDmg(int dmg) {
        base.TakeDmg(dmg);
        float ratio = (float)curHp / maxHp;
        SetScale(ratio);
    }

    public void SetScale(float scale) {
        charView.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    protected override void DoMove() {
        if(playerUnit == null) return;
        transform.position =Vector2.MoveTowards(transform.position, playerUnit.transform.position, (MOVE_SPD * Time.deltaTime)); 
    }
}