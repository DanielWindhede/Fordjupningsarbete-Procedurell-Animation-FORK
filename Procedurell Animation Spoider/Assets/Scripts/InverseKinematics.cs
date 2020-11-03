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

    float[] _bonesLength; //Target to origin
    float _completeLength; //Length of all bone lengths
    Transform[] _bones;
    Vector3[] _bonesPosition; //Safer to do math seperately from bone positions directly

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

        _completeLength = 0;

        //init bone transforms
        Transform current = transform;
        for (int i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;

            //Mid-bone (not end bone)
            if (i != _bones.Length - 1)
            {
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
        if (_target == null)
        {
            Debug.LogError("No target is set!");
            return;
        }

        //If changes are made in inspector at runtime -> Init again!
        if (_bonesLength.Length != _chainLength)
            Init();

        //Set current positions from bones
        for (int i = 0; i < _bones.Length; i++)
            _bonesPosition[i] = _bones[i].position;

        //Can leg reach target? Distance from root bone to target compared with total leg length
        //SqrMag is faster than mag 
        if ((_target.position - _bones[ROOT_BONE_INDEX].position).sqrMagnitude >= _completeLength * _completeLength)
        {
            //It can't! -> Stretch leg completely toward target
            Vector3 direction = (_target.position - _bonesPosition[ROOT_BONE_INDEX]).normalized;

            for (int i = 1; i < _bonesPosition.Length; i++)
                _bonesPosition[i] = _bonesPosition[i - 1] + direction * _bonesLength[i - 1];
        }
        //Target is close, we need to bend!
        else
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

        //move towards pole
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

        //Set calculated positions of bones after calculations
        for (int i = 0; i < _bones.Length; i++)
            _bones[i].position = _bonesPosition[i];
    }

    private void OnDrawGizmos()
    {
        //Draw target and pole gizmo
        if (_target != null)
        {
            Gizmos.color = _debugTargetColor;
            Gizmos.DrawSphere(_target.position, _debugTargetRadius);
        }
        if (_pole != null)
        {
            Gizmos.color = _debugPoleColor;
            Gizmos.DrawSphere(_pole.position, _debugPoleRadius);
        }

        //Draws wireframe of legs in editor and bone positions
        Transform current = transform;
        for (int i = 0; i < _chainLength && current != null && current.parent != null; i++)
        {
            //Draw bone gizmo
            Gizmos.color = _debugBoneColor;
            Gizmos.DrawSphere(current.position, _debugBonePositionRadius);

            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            //Makes it so the transform operations of wirecube later is from this leg perspective of origin (Or something)
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            current = current.parent;
        }
        //Last bone gizmo
        Gizmos.color = _debugBoneColor;
        Gizmos.DrawSphere(current.position, _debugBonePositionRadius);
    }
}


/*
 NOTES:

    Fix on drawqizmos magic numbers! What do they do and add them in inspector under DEBUG!
    
 */