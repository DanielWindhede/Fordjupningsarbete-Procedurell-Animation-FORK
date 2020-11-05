using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Leg
{
    [SerializeField] LegTarget _legTarget;
    [SerializeField] InverseKinematics _inverseKinematics;

    public Vector3 LeafJointPosition { get { return _inverseKinematics.LeafJointPosition; } }
    public Vector3 TargetPosition { get { return _legTarget.Position; } }

    public float SqrDistance { get { return (LeafJointPosition - TargetPosition).sqrMagnitude; } }

    public void MoveTarget()
    {
        _inverseKinematics.SetTargetPosition(TargetPosition);
    }
}

public class MoveSpider : MonoBehaviour
{
    [SerializeField, Range(0.01f, 100f)] float _maxDistance = 0.5f;
    [SerializeField] List<Leg> _legs;

    bool _isRunning = false;

    SpiderDebug _spiderDebugScript;

    private void Awake()
    {
        _spiderDebugScript = GetComponent<SpiderDebug>();
        _isRunning = true;
    }

    private void OnValidate()
    {
        _spiderDebugScript = GetComponent<SpiderDebug>();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _legs.Count; i++)
        {
            if (_legs[i].SqrDistance > _maxDistance * _maxDistance)
                _legs[i].MoveTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _spiderDebugScript.LegTargetColor;
        for (int i = 0; i < _legs.Count; i++)
            Gizmos.DrawSphere(_legs[i].TargetPosition, _spiderDebugScript.LegTargetPositionRadius);


        if (_isRunning)
        {
            Gizmos.color = _spiderDebugScript.DistanceColor;
            for (int i = 0; i < _legs.Count; i++)
                Gizmos.DrawLine(_legs[i].LeafJointPosition, _legs[i].TargetPosition);
        }
    }
}
