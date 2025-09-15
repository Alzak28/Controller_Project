using UnityEngine;
using System;
using System.Collections.Generic;

public enum PoseType { PoseA=0, PoseB=1, PoseC=2, PoseD=3 }

[RequireComponent(typeof(Animator))]
public class PlayerPoseController : MonoBehaviour
{
    [Header("Animator (opsional)")]
    public string animatorPoseParam = "PoseIndex";
    private Animator animator;

    [Header("Input")]
    public KeyCode switchPoseKey = KeyCode.E;

    [Header("Hitboxes (A–D)")]
    public List<Collider> poseHitboxes;

    [Header("Single Visual Root (opsional)")]
    public Transform visualRoot;

    [Serializable]
    public struct PoseTransform {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;
    }
    [Header("Transforms per Pose (A–D)")]
    public PoseTransform[] poseTransforms = new PoseTransform[4];

    public PoseType currentPose = PoseType.PoseA;

    void Awake() { animator = GetComponent<Animator>(); ApplyPose(); }

    void Update() { if (Input.GetKeyDown(switchPoseKey)) CyclePose(); }

    public void CyclePose() {
        currentPose = (PoseType)(((int)currentPose + 1) % 4);
        ApplyPose();
    }

    void ApplyPose() {
        // Animator (jika dipakai)
        if (animator) animator.SetInteger(animatorPoseParam, (int)currentPose);

        // Hitbox on/off
        for (int i = 0; i < poseHitboxes.Count; i++) {
            if (!poseHitboxes[i]) continue;
            bool on = (i == (int)currentPose);
            poseHitboxes[i].enabled = on;
            poseHitboxes[i].gameObject.SetActive(on);
        }

        // Ubah tampilan 1 mesh (pos/rot/scale)
        if (visualRoot && poseTransforms != null && poseTransforms.Length >= 4) {
            var t = poseTransforms[(int)currentPose];
            visualRoot.localPosition = t.localPosition;
            visualRoot.localEulerAngles = t.localEulerAngles;
            visualRoot.localScale = t.localScale;
        }
    }
}
