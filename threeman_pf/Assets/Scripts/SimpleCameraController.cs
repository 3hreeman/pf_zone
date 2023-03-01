using UnityEngine;

public class SimpleCameraController : MonoBehaviour {
        private const float OVERFLOW_DISTANCE = 5f;

        public Camera m_mainCam;
        public Transform m_target;
        public Transform m_transform;
        public Vector3 offset = new Vector3(0, 0, -10);
        public Vector3 min = new Vector3(-10, -10, -10);
        public Vector3 max = new Vector3(10, 10, -10);
        public float smoothing = 5f;

        public float m_freeMoveSpd = 3;
        public float ADJUST_SPD = 4;
        [Header("Controls")] public string XAxis = "Horizontal";
        public string YAxis = "Vertical";

        private Vector2 m_input = default(Vector2);
        private Vector2 m_velocity = default(Vector2);
        
        private float impulsePower = 0.25f;

        private void Awake() {
            var camHalfHeight = m_mainCam.orthographicSize;
            var halfWidth = camHalfHeight * m_mainCam.aspect;
        }

        void LateUpdate() {
            if (m_target != null) {
                FollowTarget();
            }
            // else {
            //     FreeMove();
            // }
        }
        
        void FollowTarget() {
            Vector3 goalPoint = m_target.position + offset;
            goalPoint.x = Mathf.Clamp(goalPoint.x, min.x, max.x);
            goalPoint.y = Mathf.Clamp(goalPoint.y, min.y, max.y);
            goalPoint.z = Mathf.Clamp(goalPoint.z, min.z, max.z);

            Vector3 nextPos;
            if (Mathf.Abs(goalPoint.x - m_transform.position.x) > OVERFLOW_DISTANCE) {
                nextPos = goalPoint;
            }
            else {
                nextPos = Vector3.Lerp(m_transform.position, goalPoint, smoothing * Time.deltaTime);
            }

            nextPos.y = goalPoint.y;
            m_transform.position = nextPos;
        }

        void FreeMove() {
            m_input = Vector2.zero;
            m_input.x = Input.GetAxis(XAxis);
            m_input.y = Input.GetAxis(YAxis);

            if (m_input != Vector2.zero) {
                m_velocity.x = m_input.x * m_freeMoveSpd * Time.deltaTime;
                m_velocity.y = m_input.y * m_freeMoveSpd * Time.deltaTime;
                var pos = m_transform.position;
                pos.x += m_velocity.x;
                pos.y += m_velocity.y;
                m_transform.position = pos;
            }
        }
}