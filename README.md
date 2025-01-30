# About PF_Zone

---

이 프로젝트는 여러가지 떠오르는 로직이나 새로운 기능들을 빠르게 검증하고 테스트해보기 위한 프로젝트입니다.
뱀서류 방식으로 등장하는 적들을 처치하는 2D 탑다운 뷰 형태의 슈팅 게임이며, 록맨처럼 일반 공격과 차징 공격을 사용할 수 있습니다.
모든 공격에는 물리효과가 적용되어, 썩 나쁘지않은 타격감을 느낄 수 있습니다.
기본적인 게임의 플레이 모습은 아래와 같습니다.

![Image](https://github.com/user-attachments/assets/f2eb087a-b255-4b30-8dd9-8e83e6b4ed5c)

현재 테스트중인 기능은 Unity의 [JOB시스템](https://unity.com/kr/blog/engine-platform/improving-job-system-performance-2022-2-part-1)입니다.

다수의 적이 등장하여 동시에 움직일 때

- MonoBehaviour의 Update
- Unity JOB시스템

두 시스템에 대한 효율성을 중점적으로 비교했습니다.

각 케이스에 대해서 오브젝트 개수를 점차로 늘려가며 테스트하며, 오브젝트 1000개가 동시에 움직이는 상황을 기준으로 다음과 같은 결과를 얻었습니다.
테스트 환경에서 구성한 뷰의 모습은 아래와 같습니다.

![Image](https://github.com/user-attachments/assets/63cf45e4-acb6-4140-9e4b-88ac119d9527)


### 1. MonoBehaviour의 Update형태로 작동할 경우

![Image](https://github.com/user-attachments/assets/62b569b4-0569-43cf-8c37-323dcdf9ea75)

1-1) Profiler의 Main Thread 내 Update문의 CPU사용량

![Image](https://github.com/user-attachments/assets/ba68ab31-48f6-4816-bbac-e548c8eb090b)
1-2) Main Thread만 열일하고 Job Thread는 놀고 있다.

### 2. Job System으로 작동할 경우

![Image](https://github.com/user-attachments/assets/02e52149-76d8-4ab4-bb40-f8999a59d873)

2-1) 1-1에 비해 Main Thread의 사용량이 현저히 줄어든 것이 보인다.

![Image](https://github.com/user-attachments/assets/f779846e-a472-44d9-8730-5b03aa7f00b2)
2-2) Main Thread에서 사용하던 연산량이 JOB Thread에서 병렬로 처리되고 있음을 확인할 수 있다.
