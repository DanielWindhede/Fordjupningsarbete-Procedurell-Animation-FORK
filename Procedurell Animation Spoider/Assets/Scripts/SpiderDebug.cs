using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDebug : MonoBehaviour
{
    [Header("Activation")]

    [SerializeField] bool _showBones = true;
    [SerializeField] bool _showLegsWireframes = true;
    [SerializeField] bool _showLegs = true;
    [SerializeField] bool _showTargets = true;
    [SerializeField] bool _showPoles = true;

    [Header("Debug Settings")]

    [SerializeField] Color _BoneColor;
    [SerializeField] Color _PoleColor;
    [SerializeField] Color _LegColor;
    [SerializeField] Color _TargetColor;
    [SerializeField] Color _LegWireframeColor = Color.white;

    [SerializeField, Range(0.01f, 10f)] float _BonePositionRadius;
    [SerializeField, Range(0.01f, 10f)] float _TargetRadius;
    [SerializeField, Range(0.01f, 10f)] float _PoleRadius;
    [SerializeField, Range(0.01f, 10f)] float _SegmentWidth;

    public bool ShowBones           { get { return _showBones; } }
    public bool ShowLegsWireframes  { get { return _showLegsWireframes; } }
    public bool ShowLegs            { get { return _showLegs; } }
    public bool ShowTargets         { get { return _showTargets; } }
    public bool ShowPoles           { get { return _showPoles; } }

    public Color BoneColor          { get { return _BoneColor; } }
    public Color PoleColor          { get { return _PoleColor; } }
    public Color LegColor           { get { return _LegColor; } }
    public Color TargetColor        { get { return _TargetColor; } }
    public Color LegWireframeColor  { get { return _LegWireframeColor; } }

    public float BonePositionRadius { get { return _BonePositionRadius; } }
    public float TargetRadius       { get { return _TargetRadius; } }
    public float PoleRadius         { get { return _PoleRadius; } }
    public float SegmentWidth       { get { return _SegmentWidth; } }
}
