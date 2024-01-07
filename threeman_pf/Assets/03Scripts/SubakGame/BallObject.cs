using System;
using UnityEngine;

public class BallObject : MonoBehaviour {
    
    public int tier;
    
    private Rigidbody2D _rigid;
    private Collider2D _col;
    private SpriteRenderer _spr;
    private void Awake() {
        _col = GetComponent<Collider2D>();
        _spr = GetComponent<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody2D>();
        // _rigid.gravityScale = transform.localScale.x;
    }

    /*public float activaDelay = .5f;
    private void Update() {
        if(activaDelay > 0) {
            activaDelay -= Time.deltaTime;
        }
    }*/

    public void Init(int tier) {
        //set object scale by tier
        isMerged = false;
        this.tier = tier;
    }
    
    public bool isMerged = false;
    private void OnCollisionEnter2D(Collision2D other) {
        if(isMerged || tier == BallGameMain.MAX_LEVEL) return;
        if (other.gameObject.CompareTag("BallObject")) {
            var otherBall = other.gameObject.GetComponent<BallObject>();
            if (otherBall.tier == tier && !otherBall.isMerged) {
                isMerged = true;
                BallGameMain.instance.MergeBall(this, otherBall);
            }
        }
    }
}
