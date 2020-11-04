using UnityEngine;
using UnityEditor;

public class InverseKinematics : MonoBehaviour
{
    [Header("Drop")]

    [SerializeField] Transform _target;
    [SerializeField] Transform _pole;

    [Header("Settings")]

    [SerializeField, Range(0, 10)] int _chainLength = 2; //Amount of segments
    [SerializeField, Range(1, 50)] int _maxIterationsPerFrame = 10; 
    [SerializeField, Range(0.0f, 1f)] float _closeEnoughToTargetDelta = 0.01f; 

    [Header("Debug")]

    [SerializeField] Color _debugBoneColor;
    [SerializeField, Range(0.01f, 10f)] float _debugBonePositionRadius;

    [SerializeField] Color _debugTargetColor;
    [SerializeField, Range(0.01f, 10f)] float _debugTargetRadius;
    [SerializeField] Color _debugPoleColor;
    [SerializeField, Range(0.01f, 10f)] float _debugPoleRadius;
    [SerializeField, Range(0.01f, 10f)] float _debugSegmentWidth;
    [SerializeField] Color _debugLegColor;
    [SerializeField] Color _debugLegWireframeColor = Color.white;

    float[] _bonesLength; //Target to origin
    float _completeLength; //Length of all bone lengths
    Transform[] _bones;
    Vector3[] _bonesPosition; //Safer to do math seperately from bone positions directly

    //Rotation stuff
    Vector3[] _startDirectionSuccesser;
    Quaternion[] _startRotationBones;
    Quaternion _startRotationTarget;
    Quaternion _startRotationRoot;

    readonly int ROOT_BONE_INDEX = 0;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        _bones = new Transform[_chainLength + 1]; //Always one more bone per chain length
        _bonesPosition = new Vector3[_chainLength + 1];
        _bonesLength = new float[_chainLength];

        _startDirectionSuccesser = new Vector3[_chainLength + 1];
        _startRotationBones = new Quaternion[_chainLength + 1];
        _startRotationTarget = _target.rotation;

        _completeLength = 0;

        //init bone transforms
        Transform current = transform;
        for (int i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBones[i] = current.rotation;

            //End bone/leaf bone
            if (i == _bones.Length - 1)
                _startDirectionSuccesser[i] = _target.position - current.position;
            //Mid-bone (not end bone)
            else
            {
                _startDirectionSuccesser[i] = _bones[i + 1].position - current.position;
                //Calculate bone length
                _bonesLength[i] = (_bones[i + 1].position - current.position).magnitude;
                _completeLength += _bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        DoInverseKinematics();
    }

    void DoInverseKinematics()
    {
        PreRunCheck();
        GetPositions();

        Quaternion rootRotation = (_bones[ROOT_BONE_INDEX].parent != null) ? _bones[ROOT_BONE_INDEX].parent.rotation : Quaternion.identity;
        Quaternion rootRotationDifference = rootRotation * Quaternion.Inverse(_startRotationRoot);

        //Can leg reach target? Distance from root bone to target compared with total leg length
        //SqrMag is faster than mag 
        if ((_target.position - _bones[ROOT_BONE_INDEX].position).sqrMagnitude >= _completeLength * _completeLength)
            Stretch(); //It can't! -> Stretch leg completely toward target
        else
            Bend(); //Target is close, we need to bend!

        AlignTowardPole();
        SetPositionsAndRotations();
    }

    void PreRunCheck()
    {
        //Can't do anything just return with error
        if (_target == null)
        {
            Debug.LogError("No target is set!");
            return;
        }

        //If changes are made in inspector at runtime -> Init again!
        if (_bonesLength.Length != _chainLength)
            Init();
    }

    void GetPositions()
    {
        //Set current positions from bones
        for (int i = 0; i < _bones.Length; i++)
            _bonesPosition[i] = _bones[i].position;
    }

    void Stretch()
    {
        Vector3 direction = (_target.position - _bonesPosition[ROOT_BONE_INDEX]).normalized;

        for (int i = 1; i < _bonesPosition.Length; i++)
            _bonesPosition[i] = _bonesPosition[i - 1] + direction * _bonesLength[i - 1];
    }

    void Bend()
    {
        for (int iteration = 0; iteration < _maxIterationsPerFrame; iteration++)
        {
            //back -> try to move leg toward target
            for (int i = _bonesPosition.Length - 1; i > 0; i--)
            {
                //Set leaf bone to target
                if (i == _bonesPosition.Length - 1)
                    _bonesPosition[i] = _target.position;
                //Moves bone along direction of previous bone but with correct length
                else
                    _bonesPosition[i] = _bonesPosition[i + 1] + (_bonesPosition[i] - _bonesPosition[i + 1]).normalized * _bonesLength[i];
            }

            //forward -> Fix so root bone is still attatched to body (Will mess up target but for each iteration it gets better)
            for (int i = 1; i < _bonesPosition.Length; i++)
                _bonesPosition[i] = _bonesPosition[i - 1] + (_bonesPosition[i] - _bonesPosition[i - 1]).normalized * _bonesLength[i - 1];

            //Close enough to target?
            if ((_bonesPosition[_bonesPosition.Length - 1] - _target.position).sqrMagnitude < _closeEnoughToTargetDelta * _closeEnoughToTargetDelta)
                break;
        }
    }

    void AlignTowardPole()
    {
        if (_pole != null)
        {
            for (int i = 1; i < _bonesPosition.Length - 1; i++)
            {
                Plane plane = new Plane(_bonesPosition[i + 1] - _bonesPosition[i - 1], _bonesPosition[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(_pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(_bonesPosition[i]);
                float angle = Vector3.SignedAngle(projectedBone - _bonesPosition[i - 1], projectedPole - _bonesPosition[i - 1], plane.normal);
                _bonesPosition[i] = Quaternion.AngleAxis(angle, plane.normal) * (_bonesPosition[i] - _bonesPosition[i - 1]) + _bonesPosition[i - 1];
            }
        }
    }

    void SetPositionsAndRotations()
    {
        //Set calculated positions of bones after calculations
        for (int i = 0; i < _bones.Length; i++)
        {
            //Set Rotation
            if (i == _bonesPosition.Length - 1)
                _bones[i].rotation = _target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBones[i];
            else
                _bones[i].rotation = Quaternion.FromToRotation(_startDirectionSuccesser[i], _bonesPosition[i + 1] - _bonesPosition[i]) * _startRotationBones[i];

            //Set position
            _bones[i].position = _bonesPosition[i];
        }
    }

    private void OnDrawGizmos()
    {
        //Draw target and pole gizmo
        if (_target != null)
            DrawPoint(_target.position, _debugTargetRadius, _debugTargetColor);
        if (_pole != null)
            DrawPoint(_pole.position, _debugPoleRadius, _debugPoleColor);

        //Draws wireframe of legs in editor and bone positions
        Transform current = transform;
        for (int i = 0; i < _chainLength && current != null && current.parent != null; i++)
        {
            //Draw bone gizmo
            DrawPoint(current.position, _debugBonePositionRadius, _debugBoneColor);

            float scale = Vector3.Distance(current.position, current.parent.position) * _debugSegmentWidth;
            //Makes it so the transform operations of wirecube later is from this leg perspective of origin (Or something)
            Matrix4x4 matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.matrix = matrix;
            Handles.color = _debugLegWireframeColor;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            Gizmos.matrix = matrix;
            Gizmos.color = _debugLegColor;
            Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;

            current = current.parent;
        }
        //Last bone gizmo
        DrawPoint(current.position, _debugBonePositionRadius, _debugBoneColor);
    }

    void DrawPoint(Vector3 position, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(position, radius);
    }
}