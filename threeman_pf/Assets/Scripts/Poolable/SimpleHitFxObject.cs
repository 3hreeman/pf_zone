using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHitFxObject : SimpleFxObject {
    public void SetDirection(Vector3 dir) {
        transform.right = dir;
    }
}
