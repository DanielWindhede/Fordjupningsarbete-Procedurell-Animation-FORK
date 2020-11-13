using UnityEngine;
using UnityEditor;

public class InverseKinematics : MonoBehaviour
{
    [Header("Drop")]

    [SerializeField] Transform _target;
    [SerializeField] Transform _pole;

    [Header("Settings")]

    [SerializeField, Range(0, 1000)] int _index;
    [SerializeField, Range(0, 10)] int _chainLength = 2; //Amount of segments
    [SerializeField, Range(1, 50)] int _maxIterationsPerFrame = 10; 
    [SerializeField, Range(0.0f, 1f)] float _closeEnoughToTargetDelta = 0.01f; 

    float[] _jointsLength; //Target to origin
    float _completeLength; //Length of all joint lengths
    Transform[] _joints;
    Vector3[] _jointsPosition; //Safer to do math seperately from joint positions directly

    //Rotation stuff
    Vector3[] _startDirectionSuccesser;
    Quaternion[] _startRotationJoints;
    Quaternion _startRotationTarget;
    Quaternion _startRotationRoot;

    Vector3 _startPosition;
    bool _isFullyStretched;

    readonly int ROOT_JOINT_INDEX = 0;
    SpiderDebug _spiderDebugScript;

    private void Awake()
    {
        _spiderDebugScript = GetComponentInParent<SpiderDebug>();
        Init();
    }

    private void OnValidate()
    {
        //Needed to show gizmos in inspector
        _spiderDebugScript = GetComponentInParent<SpiderDebug>();
    }

    public Vector3 TargetPosition { get { return _target.position; } set { _target.position = value; } }
    public Vector3 RootJointPosition { get { return _joints[ROOT_JOINT_INDEX].position; } }
    public Vector3 LeafJointPosition { get { return _joints[_joints.Length - 1].position; } }
    public bool IsStretched { get { return _isFullyStretched; } }

    void Init()
    {
        _joints = new Transform[_chainLength + 1]; //Always one more joint per chain length
        _jointsPosition = new Vector3[_chainLength + 1];
        _jointsLength = new float[_chainLength];

        _startDirectionSuccesser = new Vector3[_chainLength + 1];
        _startRotationJoints = new Quaternion[_chainLength + 1];
        _startRotationTarget = _target.rotation;

        _completeLength = 0;

        //init joint transforms
        Transform current = transform;
        for (int i = _joints.Length - 1; i >= 0; i--)
        {
            _joints[i] = current;
            _startRotationJoints[i] = current.rotation;

            //End joint/leaf joint
            if (i == _joints.Length - 1)
                _startDirectionSuccesser[i] = _target.position - current.position;
            //Mid-joint (not end joint)
            else
            {
                _startDirectionSuccesser[i] = _joints[i + 1].position - current.position;
                //Calculate joint length
                _jointsLength[i] = (_joints[i + 1].position - current.position).magnitude;
                _completeLength += _jointsLength[i];
            }

            current = current.parent;
        }

        _startPosition = _joints[ROOT_JOINT_INDEX].localPosition;
    }

    private void Update()
    {
        DoInverseKinematics();
    }

    public void DoInverseKinematics()
    {
        if (!PreRunCheck())
            return;

        GetPositions();

        Quaternion rootRotation = (_joints[ROOT_JOINT_INDEX].parent != null) ? _joints[ROOT_JOINT_INDEX].parent.rotation : Quaternion.identity;
        Quaternion rootRotationDifference = rootRotation * Quaternion.Inverse(_startRotationRoot);

        //Can leg reach target? Distance from root joint to target compared with total leg length
        //SqrMag is faster than mag 
        if ((_target.position - _joints[ROOT_JOINT_INDEX].position).sqrMagnitude >= _completeLength * _completeLength)
            Stretch(); //It can't! -> Stretch leg completely toward target
        else
            Bend(); //Target is close, we need to bend!

        AlignTowardPole();
        SetPositionsAndRotations();
    }

    bool PreRunCheck()
    {
        //Can't do anything just return with error
        if (_target == null)
        {
            Debug.LogError("No target is set!");
            return false;
        }

        //If changes are made in inspector at runtime -> Init again!
        if (_jointsLength.Length != _chainLength)
            Init();

        return true;
    }

    void GetPositions()
    {
        //Set current positions from joints
        for (int i = 0; i < _joints.Length; i++)
            _jointsPosition[i] = _joints[i].position;
    }

    void Stretch()
    {
        _isFullyStretched = true;
        Vector3 direction = (_target.position - _jointsPosition[ROOT_JOINT_INDEX]).normalized;

        for (int i = 1; i < _jointsPosition.Length; i++)
            _jointsPosition[i] = _jointsPosition[i - 1] + direction * _jointsLength[i - 1];
    }

    void Bend()
    {
        _isFullyStretched = false;

        for (int iteration = 0; iteration < _maxIterationsPerFrame; iteration++)
        {
            //back -> try to move leg toward target
            for (int i = _jointsPosition.Length - 1; i > 0; i--)
            {
                //Set leaf joint to target
                if (i == _jointsPosition.Length - 1)
                    _jointsPosition[i] = _target.position;
                //Moves joint along direction of previous joint but with correct length
                else
                    _jointsPosition[i] = _jointsPosition[i + 1] + (_jointsPosition[i] - _jointsPosition[i + 1]).normalized * _jointsLength[i];
            }

            //forward -> Fix so root joint is still attatched to body (Will mess up target but for each iteration it gets better)
            for (int i = 1; i < _jointsPosition.Length; i++)
                _jointsPosition[i] = _jointsPosition[i - 1] + (_jointsPosition[i] - _jointsPosition[i - 1]).normalized * _jointsLength[i - 1];

            //Close enough to target?
            if ((_jointsPosition[_jointsPosition.Length - 1] - _target.position).sqrMagnitude < _closeEnoughToTargetDelta * _closeEnoughToTargetDelta)
                break;
        }
    }

    void AlignTowardPole()
    {
        if (_pole != null)
        {
            for (int i = 1; i < _jointsPosition.Length - 1; i++)
            {
                Plane plane = new Plane(_jointsPosition[i + 1] - _jointsPosition[i - 1], _jointsPosition[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(_pole.position);
                Vector3 projectedJoint = plane.ClosestPointOnPlane(_jointsPosition[i]);
                float angle = Vector3.SignedAngle(projectedJoint - _jointsPosition[i - 1], projectedPole - _jointsPosition[i - 1], plane.normal);
                _jointsPosition[i] = Quaternion.AngleAxis(angle, plane.normal) * (_jointsPosition[i] - _jointsPosition[i - 1]) + _jointsPosition[i - 1];
            }
        }
    }

    void SetPositionsAndRotations()
    {
        //Set calculated positions of joints after calculations
        for (int i = 0; i < _joints.Length; i++)
        {
            //Set Rotation
            if (i == _jointsPosition.Length - 1)
                _joints[i].rotation = _target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationJoints[i];
            else
                _joints[i].rotation = Quaternion.FromToRotation(_startDirectionSuccesser[i], _jointsPosition[i + 1] - _jointsPosition[i]) * _startRotationJoints[i];

            //Set position
            _joints[i].position = _jointsPosition[i];
        }
    }

    private void OnDrawGizmos()
    {
        if (_spiderDebugScript != null)
        {
            //Draw target and pole gizmo
            if (_target != null && _spiderDebugScript.ShowTargets)
                DrawPoint(_target.position, _spiderDebugScript.TargetRadius, _spiderDebugScript.TargetColor);
            if (_pole != null && _spiderDebugScript.ShowPoles)
                DrawPoint(_pole.position, _spiderDebugScript.PoleRadius, _spiderDebugScript.PoleColor);

            //Draws wireframe of legs in editor and joint positions
            Transform current = transform;
            for (int i = 0; i < _chainLength && current != null && current.parent != null; i++)
            {
                //Draw joint gizmo
                if (_spiderDebugScript.ShowJoints)
                    DrawPoint(current.position, _spiderDebugScript.JointPositionRadius, _spiderDebugScript.JointColor);

                float scale = Vector3.Distance(current.position, current.parent.position) * _spiderDebugScript.SegmentWidth;
                //Makes it so the transform operations of wirecube later is from this leg perspective of origin (Or something)
                Matrix4x4 matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
                Handles.matrix = matrix;
                Handles.color = _spiderDebugScript.LegWireframeColor;
                if (_spiderDebugScript.ShowLegsWireframes)
                    Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

                Gizmos.matrix = matrix;
                Gizmos.color = _spiderDebugScript.LegColor;

                if (_spiderDebugScript.ShowLegs)
                    Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);

                Gizmos.matrix = Matrix4x4.identity;

                current = current.parent;
            }
            //Last joint gizmo
            if (_spiderDebugScript.ShowJoints)
                DrawPoint(current.position, _spiderDebugScript.JointPositionRadius, _spiderDebugScript.JointColor);
        }
    }

    void DrawPoint(Vector3 position, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(position, radius);
    }
}