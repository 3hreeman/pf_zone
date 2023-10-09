using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DmgFontObject : PoolingObject {
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
        { DmgTxtType.NormalAtk, new TextColorInfo(ColorMode.Single, new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(70, 72, 95, 255)) },
        { DmgTxtType.Crit, new TextColorInfo(ColorMode.VerticalGradient, new Color32(161, 10, 49, 255), new Color32(255, 54, 61, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Skill, new TextColorInfo(ColorMode.Single, new Color32(255, 128, 0, 255), new Color32(255, 128, 0, 255), new Color32(70, 72, 95, 255)) },
        { DmgTxtType.SkillCrit, new TextColorInfo(ColorMode.VerticalGradient, new Color32(255, 192, 0, 255), new Color32(255, 192, 0, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Bomb, new TextColorInfo(ColorMode.VerticalGradient, new Color32(255, 192, 0, 255), new Color32(255, 192, 0, 255), new Color32(101, 16, 16, 255)) },
        { DmgTxtType.Heal, new TextColorInfo(ColorMode.Single, new Color32(52, 106, 66, 255), new Color32(52, 106, 66, 255), new Color32(77, 238, 77, 255)) },
        { DmgTxtType.Miss, new TextColorInfo(ColorMode.VerticalGradient, new Color32(81, 83, 96, 255), new Color32(130, 131, 139, 255), new Color32(186, 189, 216, 255)) },
    };

    struct TextColorInfo {
        public ColorMode colorMode;
        public Color32 fontStartColor;
        public Color32 fontEndColor;
        public Color32 outlineColor;
        public TextColorInfo(ColorMode mode, Color32 startColor, Color32 endColor, Color32 olColor) {
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
    
    public void PrintDmgFont(Vector3 pos, string data, DmgTxtType type) {
        transform.position = pos;
        txtDmgFont.text = data;

        var colorDict = playerFont;

        TextColorInfo info = colorDict[type];
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
        _objPool.Release(this);
    }
}
