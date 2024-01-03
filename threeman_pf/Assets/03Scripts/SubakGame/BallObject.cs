using System;
using UnityEngine;

public class BallObject : MonoBehaviour {
    public const int MAX_LEVEL = 10;
    
    private float[] levelScale = new float[MAX_LEVEL] {
        1f, 1.2f, 1.4f, 1.6f, 1.8f,
        2f, 2.2f, 2.4f, 2.6f, 2.8f
    };
    
    private Color[] levelColor = new Color[MAX_LEVEL] {
        Color.white, Color.red, Color.blue, Color.green, Color.yellow,
        Color.cyan, Color.magenta, Color.gray, Color.black, Color.white
    };
    
    public int tier;
    
    private Collider2D _col;
    private SpriteRenderer _spr;
    private void Awake() {
        _col = GetComponent<Collider2D>();
        _spr = GetComponent<SpriteRenderer>();
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
        transform.localScale = Vector3.one * levelScale[tier];
        _spr.color = levelColor[tier];
    }
    
    public bool isMerged = false;
    private void OnCollisionEnter2D(Collision2D other) {
        if(isMerged || tier == MAX_LEVEL) return;
        if (other.gameObject.CompareTag("BallObject")) {
            var otherBall = other.gameObject.GetComponent<BallObject>();
            if (otherBall.tier == tier && !otherBall.isMerged) {
                isMerged = true;
                BallGameMain.instance.MergeBall(this, otherBall);
            }
        }
    }
    
}
