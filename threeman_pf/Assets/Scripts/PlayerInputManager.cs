using UnityEngine;

public class PlayerInputManager : MonoBehaviour {
    public Camera m_mainCam;
    public PlayerUnit m_player;
    
    public CombatDmgFontObject m_dmgFontPrefab;

    void Update() {
        UpdateInput();
        UpdatePlayer();
    }

    public void UpdatePlayer() {
        var inputPos = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
        m_player.UpdateAim(inputPos);

        var dirVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        m_player.UpdateDir(dirVector);

        m_player.UpdateCharging();
    }
    
    public void UpdateInput() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            m_player.DoRolling();
        }
        if (Input.GetMouseButton(0)) {
            var mousePos = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            m_player.DoAttack(mousePos);
        }

        if (Input.GetMouseButtonDown(1)) {
            m_player.SetCharging(true);
        }

        if (Input.GetMouseButtonUp(1)) {
            var mousePos = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            m_player.DoAttack(mousePos);
        }
    }
}
