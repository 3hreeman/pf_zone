using UnityEngine;

public class PlayerUnit : UnitBase {
    private const float MOVE_SPD = 5f;
    private const float DASH_TIME = 0.5f;
    private const float DASH_COOLTIME = 1f;

    private float m_leftDashTime = 0;
    private float m_nextDashTime = 0;

    [SerializeField] private WeaponObject weapon;
    [SerializeField] private CharacterView charView;
    
    private void Start() {
        weapon.Init(this);
        curHp = maxHp;
    }

    public void Update() {
        DoMove();
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

    protected override void DoMove() {
        if (dirVector == Vector3.zero) return;

        var moveSpd = MOVE_SPD;
        if (m_leftDashTime > Time.time) {
            moveSpd *= 3;
        }
        
        transform.Translate(dirVector * (moveSpd * Time.deltaTime));
    }
}
