using System;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerInputManager : MonoBehaviour {
    public Camera m_mainCam;
    public PlayerUnit m_player;
    
    public CombatDmgFontObject m_dmgFontPrefab;
    
    DmgFontPool m_dmgFontPool;

    private void Start() {
        m_dmgFontPool = new DmgFontPool(m_dmgFontPrefab.gameObject);
    }

    void Update() {
        UpdateInput();
        UpdatePlayer();
    }

    public void UpdatePlayer() {
        var inputPos = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
        m_player.UpdateAim(inputPos);

        var dirVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        m_player.UpdateDir(dirVector);
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

        if (Input.GetMouseButton(1)) {
            var mousePos = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            // CombatDmgFontObject.PrintDmgFont(mousePos, "100", CombatDmgFontObject.DmgTxtType.NormalAtk, 0);
            var font = m_dmgFontPool.Get();
            font.PrintDmgFont(mousePos, "100", CombatDmgFontObject.DmgTxtType.NormalAtk);
        }
    }
}

public class DmgFontPool {
    private GameObject prefab;
    private IObjectPool<CombatDmgFontObject> _objPool;
    public DmgFontPool(GameObject prefab) {
        this.prefab = prefab;
        _objPool = new ObjectPool<CombatDmgFontObject>(CreateObj, GetObj, ReleaseObj, DestroyObj, maxSize:100);
    }

    public CombatDmgFontObject Get() {
        return _objPool.Get();
    }
    
    public CombatDmgFontObject CreateObj() {
        var obj = GameObject.Instantiate(prefab).GetComponent<CombatDmgFontObject>();
        obj.SetPool(_objPool);
        return obj;
    }
    
    public void GetObj(CombatDmgFontObject obj) {
        obj.gameObject.SetActive(true);
    }
    
    public void ReleaseObj(CombatDmgFontObject obj) {
        obj.gameObject.SetActive(false);
    }
    
    public void DestroyObj(CombatDmgFontObject obj) {
        GameObject.Destroy(obj.gameObject);
    }
}
