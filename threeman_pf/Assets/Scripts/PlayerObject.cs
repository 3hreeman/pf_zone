

using System;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerObject : MonoBehaviour {
    public Camera mainCam;

    private const float MOVE_SPD = 5f;
    private const float DASH_TIME = 0.5f;
    private const float DASH_COOLTIME = 1f;

    private CharacterController m_charController;

    private float m_leftDashTime = 0;
    private float m_nextDashTime = 0;

    private Vector3 dirVector;

    [SerializeField] private WeaponObject weapon;
    [SerializeField] private CharacterView charView;

    private void Start() {
        m_charController = GetComponent<CharacterController>();
        weapon.Init();
    }

    public void Update() {
        UpdateInput();
        UpdateMove();
    }

    private bool CheckDashAvailable() {
        return m_nextDashTime < Time.time;
    }

    public void UpdateInput() {
        dirVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            DoRolling();
        }
        if (Input.GetMouseButton(0)) {
            DoAttack();       
        }
    }
    
    private void DoRolling() {
        if (!CheckDashAvailable()) {
            return;
        }
        m_leftDashTime = Time.time + DASH_TIME;
        m_nextDashTime = Time.time + DASH_COOLTIME;
        charView.DoRolling(dirVector, DASH_TIME);
    }
    
    private void DoAttack() {
        var end = mainCam.ScreenToWorldPoint(Input.mousePosition);
        end.z = 0;
        weapon.DoFire(transform.position, end);
    }

    public void UpdateMove() {
        var moveSpd = MOVE_SPD;
        if (m_leftDashTime > Time.time) {
            moveSpd *= 3;
        }
        
        m_charController.Move(dirVector * (moveSpd * Time.deltaTime));
    }
}
