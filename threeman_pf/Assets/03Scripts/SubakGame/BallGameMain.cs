using UnityEngine;
using Random = UnityEngine.Random;

public class BallGameMain : MonoBehaviour {
    public const int MAX_LEVEL = 10;
    public const float HEIGHT = 8f;
    public float[] tierProbability = new float[10] {
        0.3f, 0.3f, 0.2f, 0.15f, 0.1f,
        0.05f, 0.025f, 0.01f, 0.005f, 0.001f
    };
    
    public GameObject ballPrefab;
    public Transform ballParent;
    
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
        var ball = Instantiate(ballPrefab, pos, Quaternion.identity, ballParent);
        var ballObj = ball.GetComponent<BallObject>();
        ballObj.Init(tier);
        ballObj.name = $"ball_{ballNumber}";
        ballNumber++;
        return ballObj;
    }

    public void CheckPlayerInput() {
        if (Input.GetMouseButtonDown(0)) {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.y = HEIGHT;
            pos.z = 0;
            CreateBall(pos, nextBallTier);
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
        if(ball1.tier >= BallObject.MAX_LEVEL) {
            return;
        }
        var mergedTier = ball1.tier + 1;
        var pos = (ball1.transform.position + ball2.transform.position) / 2;
        Destroy(ball1.gameObject);
        Destroy(ball2.gameObject);
        var newBall = CreateBall(pos, mergedTier);
    }
    
}
