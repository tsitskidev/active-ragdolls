using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsAnimator : MonoBehaviour {
    private Transform[] _bones;
    private Dictionary<Transform, ConfigurableJoint> _configurableJoints = new();
    private Dictionary<Transform, Quaternion> _initialRotations = new();
    private Dictionary<Transform, Quaternion> _preAnimationRotations = new();
    private Dictionary<Transform, Vector3> _preAnimationPositions = new();
    private Dictionary<Transform, Quaternion> _animatedRotations = new();

    void Start() {
        _bones = transform.GetComponentsInChildren<Transform>();

        for (var i = 0; i < _bones.Length; i++) {
            if (_bones[i].TryGetComponent(out ConfigurableJoint configurableJoint)) {
                _configurableJoints[_bones[i]] = configurableJoint;
                _initialRotations[_bones[i]] = _bones[i].localRotation;
            }
        }
    }

    void Update() {
        //this stores the physics rotations, without any animation
        for (var i = 0; i < _bones.Length; i++) {
            _preAnimationRotations[_bones[i]] = _bones[i].localRotation;
            _preAnimationPositions[_bones[i]] = _bones[i].position;
        }
    }

    private void LateUpdate() {
        //this stores the animated rotations, without any physics
        for (var i = 0; i < _bones.Length; i++) {
            _animatedRotations[_bones[i]] = _bones[i].localRotation;
        }

        //this reverts the rotations and positions to the physics rotations and positions, without any animation
        for (var i = 0; i < _bones.Length; i++) {
            _bones[i].localRotation = _preAnimationRotations[_bones[i]];
            _bones[i].position = _preAnimationPositions[_bones[i]];
        }
        
        //this applies the animation through the character joints
        foreach (Transform bone in _configurableJoints.Keys) {
            _configurableJoints[bone].SetTargetRotationLocal(_animatedRotations[bone], _initialRotations[bone]);
        }
    }
}
