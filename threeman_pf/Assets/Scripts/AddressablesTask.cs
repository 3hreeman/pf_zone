using UnityEngine;

public class AddressablesTask : MonoBehaviour {
    private static AddressablesTask instance;
    private void Awake() {
        instance = this;
    }
    
    //UniTask를 이용한 Addressables 리소스 로드 처리
}
