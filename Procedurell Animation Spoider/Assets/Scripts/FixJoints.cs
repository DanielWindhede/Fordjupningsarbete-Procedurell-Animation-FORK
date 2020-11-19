using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixJoints : MonoBehaviour
{
    [SerializeField] Transform[] _jointRoots;

    Vector3[] _jointOffsets;

    private void Awake()
    {
        _jointOffsets = new Vector3[_jointRoots.Length];

        for (int i = 0; i < _jointRoots.Length; i++)
            _jointOffsets[i] = _jointRoots[i].position - transform.position;
    }

    public void DoFixedUpdate()
    {
        Vector3 bodyPosition = transform.position;
        Quaternion bodyRotation = transform.localRotation;
        for (int i = 0; i < _jointRoots.Length; i++)
            UpdateJointPositionAndRotation(bodyPosition, bodyRotation, i);
    }

    void UpdateJointPositionAndRotation(Vector3 bodyPosition, Quaternion bodyRotation, int index)
    {
        Vector3 direction = _jointOffsets[index];
        direction = bodyRotation * direction;
        _jointRoots[index].position = direction + bodyPosition;
    }
}
