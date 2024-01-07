using UnityEngine;
using Random = UnityEngine.Random;

public class BallGameMain : MonoBehaviour {
    public enum GameType {
        Basic,
        SlingShot,
    }
    
    public const int MAX_LEVEL = 9;

    public GameObject[] ballPrefabs;
    
    public float[] tierProbability = new float[9] {
        0.25f, 0.25f, 0.2f, 0.2f, 0.15f,
        0.15f, 0.1f, 0.1f, 0.05f
    };
    
    public GameType gameType = GameType.Basic;
    
    public Transform ballParent;
    public GameObject startHeightObj;    
    public Camera mainCamera;
    
    public int nextBallTier = 0;

    public static BallGameMain instance;
    private void Awake() {
        instance = this;
    }

    void Update() {
        CheckPlayerInput();
    }
    
    public int ballNumber = 0; 

    public BallObject CreateBall(Vector3 pos, int tier) {
        var prefab = ballPrefabs[tier];
        var ball = Instantiate(prefab, pos, Quaternion.identity, ballParent);
        var ballObj = ball.GetComponent<BallObject>();
        ballObj.Init(tier);
        ballObj.name = $"ball_{ballNumber}";
        ballNumber++;
        return ballObj;
    }

    public void SetRandomForce(BallObject ball) {
        var force = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        ball.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }
    
    public void CheckPlayerInput() {
        if (Input.GetMouseButtonDown(0)) {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.y = startHeightObj.transform.position.y;
            pos.z = 0;

            GenerateBall(pos);
        }else if (Input.GetMouseButton(1)) {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            GenerateBall(pos);
        }
    }
    
    private void GenerateBall(Vector3 pos) {

        if(gameType == GameType.Basic) {
            CreateBall(pos, nextBallTier);
            nextBallTier = GetRandomTier();
        }else if(gameType == GameType.SlingShot) {
            pos.z = 0;
            var newBall = CreateBall(pos, nextBallTier);
            SetRandomForce(newBall);
            nextBallTier = GetRandomTier();
        }
    }
    
    public int GetRandomTier() {
        //get random tier by probability array
        for(int i=0; i<tierProbability.Length; i++) {
            //check probability
            if(tierProbability[i] > Random.value) {
                return i;
            }
        }

        return 0;
    }
    
    /*public void MergeBall(BallObject ball1, BallObject ball2) {
        var mergedTier = ball1.tier + 1;
        if(mergedTier >= BallObject.MAX_LEVEL) {
            return;
        }
        var pos = (ball1.transform.position + ball2.transform.position) / 2;
        Destroy(ball1.gameObject);
        Destroy(ball2.gameObject);
        var newBall = CreateBall(pos, mergedTier);
        
    }*/
    public void MergeBall(BallObject ball1, BallObject ball2) {
        if(ball1.tier >= MAX_LEVEL-1) {
            return;
        }
        var mergedTier = ball1.tier + 1;
        var pos = (ball1.transform.position + ball2.transform.position) / 2;
        Destroy(ball1.gameObject);
        Destroy(ball2.gameObject);
        var newBall = CreateBall(pos, mergedTier);
    }
    
}
