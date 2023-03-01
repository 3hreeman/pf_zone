

using System;
using UnityEngine;

public class PlayerObject : MonoBehaviour {
    private const float MOVE_SPD = 3f;
    private const float DASH_TIME = 0.5f;
    private const float DASH_COOLTIME = 1f;

    private CharacterController m_charController;
    
    private float m_leftDashTime = 0;
    private float m_nextDashTime = 0;
    private void Start() {
        
        m_charController = GetComponent<CharacterController>();
    }

    public void Update() {
        UpdateInput();
    }

    private bool CheckDashAvailable() {
        return m_nextDashTime < Time.time;
    }

    public void UpdateInput() {
        var moveSpd = MOVE_SPD;
        Vector2 moveVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift) && CheckDashAvailable()) {
            m_leftDashTime = Time.time + DASH_TIME;
            m_nextDashTime = Time.time + DASH_COOLTIME;
        }
        if (m_leftDashTime > Time.time) {
            moveSpd *= 2;
        }
        
        m_charController.Move(moveVector * (moveSpd * Time.deltaTime));
    }
}
