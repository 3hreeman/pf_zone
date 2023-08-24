using UnityEngine;

public class SimpleFxObject : PoolingObject {
    public float releaseAt = 0;

    public override void OnTakeObject(PoolingObject pObj) {
        var simpleFx = pObj as SimpleFxObject;
        simpleFx.releaseAt = Time.time + 2;
        base.OnTakeObject(pObj);
    }

    // Update is called once per frame
    void Update() {
        if(Time.time > releaseAt)
            ReleaseObject();
    }
}
