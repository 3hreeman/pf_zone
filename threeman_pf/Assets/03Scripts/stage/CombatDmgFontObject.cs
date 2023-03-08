using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class CombatDmgFontObject : MonoBehaviour {
    public static GameObject DmgFontPrefab = null;
    static Queue<CombatDmgFontObject> entryQueue = new Queue<CombatDmgFontObject>();
    static List<CombatDmgFontObject> usingList = new List<CombatDmgFontObject>();

    public TextMeshPro txtDmgFont;
    public Animator anim;

    public enum DmgTxtType {
        NormalAtk,
        Skill,
        Crit,
        SkillCrit,
        Bomb,
        Miss,
        Heal
    }

    static Dictionary<DmgTxtType, TextColorInfo> playerFont = new Dictionary<DmgTxtType, TextColorInfo> {
        { DmgTxtType.NormalAtk, new TextColorInfo(2, ColorMode.Single, new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(70, 72, 95, 255)) },
        { DmgTxtType.Crit, new TextColorInfo(2.5f, ColorMode.VerticalGradient, new Color32(161, 10, 49, 255), new Color32(255, 54, 61, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Skill, new TextColorInfo(2.5f, ColorMode.Single, new Color32(255, 128, 0, 255), new Color32(255, 128, 0, 255), new Color32(70, 72, 95, 255)) },
        { DmgTxtType.SkillCrit, new TextColorInfo(3f, ColorMode.VerticalGradient, new Color32(255, 192, 0, 255), new Color32(255, 192, 0, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Bomb, new TextColorInfo(3f, ColorMode.VerticalGradient, new Color32(255, 192, 0, 255), new Color32(255, 192, 0, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Heal, new TextColorInfo(1.5f, ColorMode.Single, new Color32(52, 106, 66, 255), new Color32(52, 106, 66, 255), new Color32(77, 238, 77, 255)) },
        { DmgTxtType.Miss, new TextColorInfo(1.25f, ColorMode.VerticalGradient, new Color32(81, 83, 96, 255), new Color32(130, 131, 139, 255), new Color32(186, 189, 216, 255)) },
    };

    struct TextColorInfo {
        public float size;
        public ColorMode colorMode;
        public Color32 fontStartColor;
        public Color32 fontEndColor;
        public Color32 outlineColor;
        public TextColorInfo(float size, ColorMode mode, Color32 startColor, Color32 endColor, Color32 olColor) {
            this.size = size;
            colorMode = mode;
            fontStartColor = startColor;
            fontEndColor = endColor;
            outlineColor = olColor;
        }
    }

    private void Awake() {
        txtDmgFont.text = "NOT_SETTED";
        gameObject.SetActive(false);
    }

    static void LoadDmgObjCallback(object a, object b) {
        
    }
    public static void PrintDmgFont(Vector3 pos, string data, DmgTxtType type, int order_idx) {
        //return;
        CombatDmgFontObject combatDmgFont = null;
        if(entryQueue.Count == 0) {
            if (DmgFontPrefab == null) {
                DmgFontPrefab = Resources.Load<GameObject>("Combat/CombatDmgTextPrefab");
            }
            combatDmgFont = Instantiate(DmgFontPrefab).GetComponent<CombatDmgFontObject>();
            // SimpleInstantEffManager.AttachToEffectLayer(dmg_font.gameObject);
        }
        else {
            combatDmgFont = entryQueue.Dequeue();
        }
        combatDmgFont.transform.position = pos;
        combatDmgFont.txtDmgFont.text = data;
        usingList.Add(combatDmgFont);

        combatDmgFont.gameObject.SetActive(true);

        var colorDict = playerFont;

        TextColorInfo info = colorDict[type];
        combatDmgFont.txtDmgFont.fontSize = info.size;
        combatDmgFont.txtDmgFont.colorGradient = new VertexGradient(info.fontStartColor, info.fontStartColor, info.fontEndColor, info.fontEndColor);
        combatDmgFont.txtDmgFont.outlineColor = info.outlineColor;

        combatDmgFont.txtDmgFont.sortingOrder = order_idx;
        if (type == DmgTxtType.Crit || type == DmgTxtType.SkillCrit) {
            combatDmgFont.anim.SetTrigger("critical");
        }
        else if (type == DmgTxtType.Bomb) {
            combatDmgFont.anim.SetTrigger("bomb");    
        }
        else if (type == DmgTxtType.Heal) {
            combatDmgFont.anim.SetTrigger("heal");
        }
        else if (type == DmgTxtType.Miss) {
            combatDmgFont.anim.SetTrigger("normal");
        }
        else {
            combatDmgFont.anim.SetTrigger("normal");
        }
    }
    
    public void PrintDmgFont(Vector3 pos, string data, DmgTxtType type) {
        transform.position = pos;
        txtDmgFont.text = data;

        var colorDict = playerFont;

        TextColorInfo info = colorDict[type];
        txtDmgFont.fontSize = info.size;
        txtDmgFont.colorGradient = new VertexGradient(info.fontStartColor, info.fontStartColor, info.fontEndColor, info.fontEndColor);
        txtDmgFont.outlineColor = info.outlineColor;

        txtDmgFont.sortingOrder = 0;
        if (type == DmgTxtType.Crit || type == DmgTxtType.SkillCrit) {
            anim.SetTrigger("critical");
        }
        else if (type == DmgTxtType.Bomb) {
            anim.SetTrigger("bomb");    
        }
        else if (type == DmgTxtType.Heal) {
            anim.SetTrigger("heal");
        }
        else if (type == DmgTxtType.Miss) {
            anim.SetTrigger("normal");
        }
        else {
            anim.SetTrigger("normal");
        }
    }

    public void OnAnimationEnd() {
        if (usingPool) {
            _objPool.Release(this);
        }
        else {
            usingList.Remove(this);
            entryQueue.Enqueue(this);
            gameObject.SetActive(false);
        }
    }

    public IObjectPool<CombatDmgFontObject> _objPool;

    private bool usingPool = false;
    public void SetPool(IObjectPool<CombatDmgFontObject> pool) {
        usingPool = true;
        _objPool = pool;
    }
}

