using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpiderDebug : MonoBehaviour
{
    [Header("Activation")]

    [SerializeField] bool _showBones = true;
    [SerializeField] bool _showLegsWireframes = true;
    [SerializeField] bool _showLegs = true;
    [SerializeField] bool _showTargets = true;
    [SerializeField] bool _showPoles = true;

    [Header("Debug Settings")]

    [SerializeField] Vector3 _moveSpiderHandleStartPosition;

    [SerializeField] Color _BoneColor;
    [SerializeField] Color _PoleColor;
    [SerializeField] Color _LegColor;
    [SerializeField] Color _TargetColor;
    [SerializeField] Color _LegWireframeColor = Color.white;

    [SerializeField, Range(0.01f, 10f)] float _BonePositionRadius;
    [SerializeField, Range(0.01f, 10f)] float _TargetRadius;
    [SerializeField, Range(0.01f, 10f)] float _PoleRadius;
    [SerializeField, Range(0.01f, 10f)] float _SegmentWidth;

    [Header("Drop")]

    [SerializeField] Transform[] _moveSpiderTransforms;

    Vector3[] _moveSpiderOffsets;
    Vector3 _currentMoveSpiderAxis;

    public bool ShowBones                        { get { return _showBones; } }
    public bool ShowLegsWireframes               { get { return _showLegsWireframes; } }
    public bool ShowLegs                         { get { return _showLegs; } }
    public bool ShowTargets                      { get { return _showTargets; } }
    public bool ShowPoles                        { get { return _showPoles; } }

    public Vector3 MoveSpiderHandleStartPosition { get { return _moveSpiderHandleStartPosition; } }
    public Vector3 CurrentMoveSpiderAxisPosition { get { return _currentMoveSpiderAxis; } set { _currentMoveSpiderAxis = value; } }

    public Color BoneColor                       { get { return _BoneColor; } }
    public Color PoleColor                       { get { return _PoleColor; } }
    public Color LegColor                        { get { return _LegColor; } }
    public Color TargetColor                     { get { return _TargetColor; } }
    public Color LegWireframeColor               { get { return _LegWireframeColor; } }
                                                 
    public float BonePositionRadius              { get { return _BonePositionRadius; } }
    public float TargetRadius                    { get { return _TargetRadius; } }
    public float PoleRadius                      { get { return _PoleRadius; } }
    public float SegmentWidth                    { get { return _SegmentWidth; } }

    private void Awake()
    {
        SetupMoveSpider();
    }

    private void OnValidated()
    {
        SetupMoveSpider();
    }

    void SetupMoveSpider()
    {
        _currentMoveSpiderAxis = _moveSpiderHandleStartPosition;

        _moveSpiderOffsets = new Vector3[_moveSpiderTransforms.Length];
        for (int i = 0; i < _moveSpiderTransforms.Length; i++)
            _moveSpiderOffsets[i] = _moveSpiderHandleStartPosition - _moveSpiderTransforms[i].position;
    }

    public void MoveSpider(Vector3 axisPosition)
    {
        for (int i = 0; i < _moveSpiderTransforms.Length; i++)
            _moveSpiderTransforms[i].position = axisPosition - _moveSpiderOffsets[i];
    }
}
