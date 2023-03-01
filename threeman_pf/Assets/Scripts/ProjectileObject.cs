using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MovingObject {
    static Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    static Dictionary<string, Queue<ProjectileObject>> entryDict = new Dictionary<string, Queue<ProjectileObject>>();
    static Dictionary<string, List<ProjectileObject>> usingDict = new Dictionary<string, List<ProjectileObject>>();
    private static int MAKE_COUNT = 3;
    
    [SerializeField] public AnimationCurve x_curve;
    [SerializeField] public AnimationCurve y_curve;
    
    public float MAX_HOVER_HEIGHT = 1;
    private Action endCallback = null;
    private string res_key;
    public static ProjectileObject GetProjectileFromPool(string key) {
        if (entryDict.ContainsKey(key) == false) {
            entryDict.Add(key, new Queue<ProjectileObject>());
            usingDict.Add(key, new List<ProjectileObject>());
        }

        var entryQueue = entryDict[key];
        var usingList = usingDict[key];
        
        if (entryQueue.Count <= 0) {
            // Debug.Log("make projectile entry");
            if (prefabDict.ContainsKey(key) == false) {
                var res = Resources.Load<GameObject>(key);
                if (res != null) {
                    prefabDict.Add(key, res);
                }
                else {
                    Debug.Log("ProjectileObject :: "+key + " is NULL");
                    return null;
                }
            }

            var prefab = prefabDict[key];

            for (int i = 0; i < MAKE_COUNT; i++) {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                var po = obj.GetComponent<ProjectileObject>();
                po.res_key = key;
                entryQueue.Enqueue(po);
            }
        }

        var entry = entryQueue.Dequeue();
        usingList.Add(entry);
        return entry;
    }
    
    public void init(Vector3 start_pos, Vector3 end_pos, Action endCallback) {
        this.start_pos = start_pos;
        this.end_pos = end_pos;
        gameObject.SetActive(true);
        transform.right = Vector3.right;
        this.endCallback = endCallback;
        StartCoroutine(MoveCoroutine());
    }

    void uninit() {
        gameObject.SetActive(false);
        usingDict[res_key].Remove(this);
        entryDict[res_key].Enqueue(this);
    }

    private IEnumerator MoveCoroutine() {
        float hover_height = MAX_HOVER_HEIGHT;
        
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

        if (endCallback != null) {
            endCallback();
        }
        uninit();
    }
}