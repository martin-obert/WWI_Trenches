using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour {

	// Use this for initialization
    public Animator animator;

    [Range(0,1)]
    public float LeftHandWeight;
    public Transform LeftHandTarget;

	void OnAnimatorIK() {
	    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftHandWeight);
	    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftHandWeight);
	    animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
	    animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
    }
}
