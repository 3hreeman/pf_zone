using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ShotObject : MonoBehaviour {
#region pooling code
    private IObjectPool<ShotObject> _objPool;
    
    public void SetPool(IObjectPool<ShotObject> pool) {
        _objPool = pool;
    }
    
    public void ReleaseObject() {
        _objPool.Release(this);
    }
#endregion

    private const float MAX_HOVER_HEIGHT = 3f;
    
    [SerializeField] public AnimationCurve x_curve;
    [SerializeField] public AnimationCurve y_curve;
    
    [SerializeField]
    private Transform obj_transform;
    [SerializeField]
    private Transform shot_transform;
    
    private Vector3 start_pos;
    private Vector3 end_pos;
    private Vector3 dir;
    private float move_spd = 10;
    
    private Action endCallback = null;
    private string res_key;

    private static GameObject playerObj;
    
    public void ShotStart(Vector3 start, Vector3 end, Action callback = null) {
        start_pos = start;
        end_pos = end;
        dir = (end - start).normalized;
        dir.z = 0;
        transform.position = start;
        endCallback = callback;
        
        // StartCoroutine(MoveCoroutine());
        Invoke("ShotEnd", 2f);
    }

    private void ShotEnd() {
        if (endCallback != null) {
            endCallback();
        }
        gameObject.SetActive(false);
        ReleaseObject();
    }

    void Update() {
        transform.Translate(dir * (Time.deltaTime * move_spd));
        shot_transform.right = dir;
    }
    
    private IEnumerator MoveCoroutine() {
        float dist = Vector3.Distance(start_pos, end_pos);
        float duration = dist / move_spd;
        float time = 0.0f;
        
        while(time < duration) {
            time += Time.deltaTime;
            Vector2 cur_pos = obj_transform.position;

            float time_ratio = time / duration;

            float x_eval = x_curve.Evaluate(time_ratio);
            float y_eval = y_curve.Evaluate(time_ratio);

            float x_pos = Mathf.Lerp(start_pos.x, end_pos.x, x_eval);
            float y_pos = Mathf.Lerp(start_pos.y, end_pos.y, time_ratio);
            float y_diff = Mathf.Lerp(0, MAX_HOVER_HEIGHT, y_eval);
            Vector2 pos = new Vector2(x_pos, y_pos+y_diff);
                
            var next_pos = pos;
            var dir = next_pos - cur_pos;
            shot_transform.right = dir.normalized;
            obj_transform.position = next_pos;

            yield return null;
        }

        ShotEnd();
    }
}