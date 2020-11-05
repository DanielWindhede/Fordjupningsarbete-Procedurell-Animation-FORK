using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Leg
{
    [SerializeField] LegTarget _legTarget;
    [SerializeField] InverseKinematics _inverseKinematics;

    Vector3 _startMovingJointPosition;
    float _currentMoveFraction;

    public static float MoveSpeed { get; set; }

    public Vector3 LeafJointPosition { get { return _inverseKinematics.LeafJointPosition; } }
    public Vector3 TargetPosition { get { return _legTarget.Position; } }
    public bool Moving { get; private set; }

    public float SqrDistance { get { return (LeafJointPosition - TargetPosition).sqrMagnitude; } }

    public void UpdateInverseKinematics()
    {
        _inverseKinematics.DoInverseKinematics();
    }

    public void StartMoving()
    {
        Moving = true;
        _startMovingJointPosition = LeafJointPosition;
        _currentMoveFraction = 0;
    }

    public void Move()
    {
        _currentMoveFraction += Time.deltaTime * MoveSpeed;
        Vector3 newPosition = Vector3.Lerp(_startMovingJointPosition, TargetPosition, _currentMoveFraction);
        _inverseKinematics.SetTargetPosition(newPosition);

        if (_currentMoveFraction >= 1.0f)
            Moving = false;
    }

    public void Rotate(Vector3 pivot, Quaternion rotation)
    {
        _inverseKinematics.Rotate(pivot, rotation);
    }
}

public class MoveSpider : MonoBehaviour
{
    [SerializeField] Transform _body;
    [SerializeField, Range(0.01f, 100f)] float _moveSpeed = 1f;
    [SerializeField, Range(0.01f, 100f)] float _maxDistance = 0.5f;
    [SerializeField] List<Leg> _legs;

    bool _isRunning = false;
    Quaternion _lastRotation;

    SpiderDebug _spiderDebugScript;

    private void Awake()
    {
        _spiderDebugScript = GetComponent<SpiderDebug>();
        Leg.MoveSpeed = _moveSpeed;
        _lastRotation = _body.localRotation;
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
            if (_legs[i].SqrDistance > _maxDistance * _maxDistance && !_legs[i].Moving)
                _legs[i].StartMoving();
            if (_legs[i].Moving)
                _legs[i].Move();

            if (_lastRotation != _body.localRotation)
                _legs[i].Rotate(_body.transform.position, _body.transform.localRotation);
        }

        _lastRotation = _body.localRotation;
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
