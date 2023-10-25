using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIKController : MonoBehaviour
{
    RigBuilder rigBuilder;
    Animator anim;

    [Header("Hand Constraints")]
    [SerializeField] TwoBoneIKConstraint rightHandConstraint;
    [SerializeField] TwoBoneIKConstraint leftHandConstraint;
    [Space]
    [SerializeField] Transform rightHandTarget;
    [SerializeField] Transform rightHint;
    [Space]
    [SerializeField] Transform leftHandTarget;
    [SerializeField] Transform leftHint;

    Transform rightHandRef, leftHandRef;

    private void Start() {
        anim = GetComponentInParent<Animator>();
        rigBuilder = GetComponentInParent<RigBuilder>();
        SetConstraints();
    }

    public void SetHandGripPos(Transform frontGrip, Vector3 leftHintPos, Transform backGrip, Vector3 rightHintPos) {
        leftHandRef = frontGrip;
        rightHandRef = backGrip;

        rightHint.localPosition = rightHintPos;
        leftHint.localPosition = leftHintPos;
    }

    private void LateUpdate() {
        SetTargetToRef();
    }

    private void SetConstraints() {

        //Right hand IK Constraints
        rightHandConstraint.data.root = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        rightHandConstraint.data.mid = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        rightHandConstraint.data.tip = anim.GetBoneTransform(HumanBodyBones.RightHand);

        //Left hand IK Constraints
        leftHandConstraint.data.root = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        leftHandConstraint.data.mid = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftHandConstraint.data.tip = anim.GetBoneTransform(HumanBodyBones.LeftHand);

        rightHandConstraint.CreateJob(anim);
        leftHandConstraint.CreateJob(anim);

        Rebuild();
    }

    private void SetTargetToRef() {
        if (rightHandRef != null) {
            rightHandTarget.position = rightHandRef.position;
            rightHandTarget.rotation = rightHandRef.rotation;
        }

        if (leftHandRef != null) {
            leftHandTarget.position = leftHandRef.position;
            leftHandTarget.rotation = leftHandRef.rotation;
        }
    }

    public void SetHandIKWeight(float handR = 1, float handL = 1) {
        rightHandConstraint.weight = handR;
        leftHandConstraint.weight = handL;
    }

    private void Rebuild() {
        rigBuilder.Build();
        anim.Rebind();
    }
}
