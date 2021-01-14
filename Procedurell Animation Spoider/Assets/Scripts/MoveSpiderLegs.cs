using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Leg
{
    [SerializeField] LegTarget _legTarget;
    [SerializeField] InverseKinematics _inverseKinematics;
    //Used to get Legs that must be grounded
    [SerializeField] LegTarget[] _mustBeGroundedLegTargets;

    List<Leg> _mustBeGroundedLegs;

    Vector3 _startMovingJointPosition;
    Vector3 _virtualTargetPosition;
    float _currentMoveFraction;

    public Vector3 LeafJointPosition { get { return _inverseKinematics.LeafJointPosition; } }
    public Vector3 TargetPosition { get { return _legTarget.Position; } }
    public Vector3 VirtualTargetPosition { get { return _legTarget.VirtualPosition; } }
    public bool Moving { get; private set; }
    public bool Stretched { get { return _inverseKinematics.IsStretched; } }
    public Vector3 Normal { get { return _legTarget.Normal; } }

    public float SqrDistance { get { return (LeafJointPosition - VirtualTargetPosition).sqrMagnitude; } }

    public void SetupOppositeLegs(List<Leg> allLegs)
    {
        _mustBeGroundedLegs = new List<Leg>();
        for (int i = 0; i < allLegs.Count; i++)
        {
            for (int j = 0; j < _mustBeGroundedLegTargets.Length; j++)
            {
                if (allLegs[i]._legTarget == _mustBeGroundedLegTargets[j])
                    _mustBeGroundedLegs.Add(allLegs[i]);
            }
        }

        _legTarget.DoFixedUpdate();
        _virtualTargetPosition = VirtualTargetPosition;
    }

    //If any of the opposite legs is moving this leg can't move!
    //Usually only 1 leg but support exists for multiple
    public bool CanStartMoving
    {
        get
        {
            for (int i = 0; i < _mustBeGroundedLegs.Count; i++)
            {
                if (_mustBeGroundedLegs[i].Moving)
                    return false;
            }

            return true;
        } 
    }

    public void UpdateInverseKinematics()
    {
        _inverseKinematics.DoInverseKinematics();
    }

    public void StartMoving(float maxDistance)
    {
        _startMovingJointPosition = LeafJointPosition;

        if (SqrDistance < maxDistance * maxDistance)
        {
            Vector3 direction = (VirtualTargetPosition - _startMovingJointPosition).normalized;
            _virtualTargetPosition = _startMovingJointPosition + direction * maxDistance;
        }
        else
            _virtualTargetPosition = VirtualTargetPosition;

        Moving = true;
        _currentMoveFraction = 0;
    }

    public void Move(float moveSpeed, float legHeight)
    {
        if (_currentMoveFraction >= 1.0f)
        {
            Moving = false;
            return;
        }

        float startDistance = (_startMovingJointPosition - _virtualTargetPosition).sqrMagnitude;
        float actualDistance = (LeafJointPosition - _virtualTargetPosition).sqrMagnitude;
        float speedFraction = Mathf.Clamp(actualDistance / startDistance, 1.0f, actualDistance / startDistance);

        _currentMoveFraction += Time.deltaTime * moveSpeed * speedFraction;
        Vector3 newPosition = Vector3.Lerp(_startMovingJointPosition, _virtualTargetPosition, _currentMoveFraction);
        newPosition += _inverseKinematics.transform.up * Lerp3(0, legHeight, 0, _currentMoveFraction);
        _inverseKinematics.TargetPosition = newPosition;
    }

    float Lerp3(float a, float b, float c, float t)
    {
        if (t <= 0.5f)
            return Mathf.SmoothStep(a, b, t * 2); // perform task in half the time
        else
            return Mathf.SmoothStep(b, c, t * 2 - 1);
    }
}

public class MoveSpiderLegs : MonoBehaviour
{
    [SerializeField] Transform _body;
    [SerializeField, Range(0.01f, 10f)] float _bodyHeightOffset = 1.2f;
    [SerializeField, Range(0.01f, 100f)] float _minimumLegSpeed = 1f;
    [SerializeField, Range(0.01f, 100f)] float _legHeight = 1f;
    [SerializeField, Range(0.01f, 100f)] float _maxDistance = 0.5f;
    [SerializeField, Range(0.01f, 10f)] float _virtualLegTargetRadius;
    [SerializeField, Range(0.01f, 10f)] float _correctBodyHeightInfluenceLength = 0.75f;
    [SerializeField, Range(0.01f, 10f)] float _correctPositionStrength = 4.0f;
    [SerializeField, Range(0.01f, 10f)] float _minimumBodyCorrectionStrength = 0.6f;
    [SerializeField, Range(0.01f, 10f)] float _maximumBodyCorrectionStrength = 6.0f;
    [SerializeField, Range(0.01f, 10f)] float _rotateBodyStrength = 1.0f;
    [SerializeField] List<Leg> _legs;

    bool _isRunning = false;
    Vector3 _averageLegPosition;

    ActorControl _actorControl;
    public float VirtualLegTargetRadius { get { return _virtualLegTargetRadius; } }

    private void Start()
    {
        _actorControl = GetComponent<ActorControl>();

        for (int i = 0; i < _legs.Count; i++)
            _legs[i].SetupOppositeLegs(_legs);

        _isRunning = true;
    }

    private void OnValidate()
    {
        _actorControl = GetComponent<ActorControl>();
    }

    public void DoFixedUpdate()
    {
        Vector3 addedLegPositions = Vector3.zero;
        Vector3 addedLegNormals = Vector3.zero;

        for (int i = 0; i < _legs.Count; i++)
        {
            addedLegPositions += _legs[i].LeafJointPosition;
            addedLegNormals += _legs[i].Normal;

            if ((_legs[i].SqrDistance > _maxDistance * _maxDistance || _legs[i].Stretched) && !_legs[i].Moving && _legs[i].CanStartMoving)
                _legs[i].StartMoving(_maxDistance);

            if (_legs[i].Moving)
                _legs[i].Move(_minimumLegSpeed, _legHeight);
        }

        Vector3 averageLegPosition = addedLegPositions / _legs.Count;
        Vector3 averageLegNormal = addedLegNormals / _legs.Count;

        SetBodyHeight(averageLegPosition);
        RotateBody(averageLegNormal);
    }

    void SetBodyHeight(Vector3 averageLegPosition)
    {
        _averageLegPosition = averageLegPosition + _body.up * _bodyHeightOffset;
        float delta = Time.deltaTime * Mathf.Clamp(_correctPositionStrength * (_body.position - _averageLegPosition).sqrMagnitude, _minimumBodyCorrectionStrength, _maximumBodyCorrectionStrength);
        _body.position = Vector3.MoveTowards(_body.position, _averageLegPosition, delta);
    }

    void RotateBody(Vector3 averageLegNormal)
    {
        //_body.rotation = Quaternion.Lerp(_body.localRotation, _body.localRotation * Quaternion.FromToRotation(_body.up, averageLegNormal), Time.fixedDeltaTime * _rotateBodyStrength);

        _body.up = Vector3.MoveTowards(_body.up, averageLegNormal, Time.fixedDeltaTime * 1f);
        _body.transform.Rotate(Vector3.up, _actorControl.ControlSpider.Rotation);

        //_body.up = Vector3.MoveTowards(_body.up, averageLegNormal, Time.fixedDeltaTime * 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (_isRunning)
        {
            if (_actorControl.SpiderDebug.ShowDistanceToTarget)
            {
                Gizmos.color = _actorControl.SpiderDebug.DistanceColor;
                for (int i = 0; i < _legs.Count; i++)
                    Gizmos.DrawLine(_legs[i].LeafJointPosition, _legs[i].TargetPosition);
            }
            if (_actorControl.SpiderDebug.ShowAverageLegPosition)
            {
                Gizmos.color = _actorControl.SpiderDebug.AverageLegPositionColor;
                Gizmos.DrawSphere(_averageLegPosition, _actorControl.SpiderDebug.AverageLegPositionRadius);
            }
            if (_actorControl.SpiderDebug.ShowAverageLegPositionSphere)
            {
                Gizmos.color = _actorControl.SpiderDebug.AverageLegPositionSphereColor;
                Gizmos.DrawSphere(_body.position, _correctBodyHeightInfluenceLength);
            }
            if (_actorControl.SpiderDebug.ShowDistanceFromAverageLegPosition)
            {
                Gizmos.color = _actorControl.SpiderDebug.DistanceFromAverageLegPositionColor;
                Gizmos.DrawLine(_body.position, _averageLegPosition);
            }
        }
    }
}
