using System;
using System.Collections;
using UnityEngine;

public class CharacterView : MonoBehaviour {
    [SerializeField] private ParticleSystem shadowParticle;
    private ParticleSystem.MainModule shadowParticleMain;
    [SerializeField] private Transform viewObject;

    public Vector2 lastDir;

    private void Awake() {
        shadowParticleMain = shadowParticle.main;
    }

    public void DoRolling(Vector2 dir, float time) {
        if (!dir.Equals(lastDir)) {
            lastDir = dir;
        }
        float angle = lastDir.x > 0 ? -720 : 720;
        StartCoroutine(DoRotate(angle, time));
    }

    private IEnumerator DoRotate(float angle, float time) {
        PlayShadow(true);
        float tick = 0;
        while (tick < time) {
            tick += Time.deltaTime;
            viewObject.Rotate(0, 0, angle * Time.deltaTime);
            shadowParticleMain.startRotation = -viewObject.eulerAngles.z * Mathf.Deg2Rad;
            yield return null;
        }

        viewObject.eulerAngles = new Vector3(0, 0, 0);
        shadowParticleMain.startRotation = new ParticleSystem.MinMaxCurve();
        PlayShadow(false);
    }
    
    private void PlayShadow(bool flag) {
        if(flag) {
            shadowParticle.Play();
        } else {
            shadowParticle.Stop();
        }
    }
}
