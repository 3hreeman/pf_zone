using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgFontSubObject : MonoBehaviour
{
    public CombatDmgFontObject dmgFont;
    public DmgFontObject dmgFontObj;
    public void OnAnimationEnd() {
        if (dmgFont != null) {
            dmgFont.OnAnimationEnd();
        }
        else if (dmgFontObj != null) {
            dmgFontObj.OnAnimationEnd();
        }
    }
}
