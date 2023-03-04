using UnityEngine;

public class MovingObject : MonoBehaviour {
    public Transform obj_transform;
    public Transform shot_transform;
    public float move_spd = 3;
    public Vector3 start_pos;
    public Vector3 end_pos;

    public virtual void Move() { }
}