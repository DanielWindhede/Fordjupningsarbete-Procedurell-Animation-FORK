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

    [SerializeField] Color _debugBoneColor;
    [SerializeField] Color _debugPoleColor;
    [SerializeField] Color _debugLegColor;
    [SerializeField] Color _debugTargetColor;
    [SerializeField] Color _debugLegWireframeColor = Color.white;

    [SerializeField, Range(0.01f, 10f)] float _debugBonePositionRadius;
    [SerializeField, Range(0.01f, 10f)] float _debugTargetRadius;
    [SerializeField, Range(0.01f, 10f)] float _debugPoleRadius;
    [SerializeField, Range(0.01f, 10f)] float _debugSegmentWidth;

    public bool ShowBones                { get { return _showBones; } }
    public bool ShowLegsWireframes       { get { return _showLegsWireframes; } }
    public bool ShowLegs                 { get { return _showLegs; } }
    public bool ShowTargets              { get { return _showTargets; } }
    public bool ShowPoles                { get { return _showPoles; } }

    public Color DebugBoneColor          { get { return _debugBoneColor; } }
    public Color DebugPoleColor          { get { return _debugPoleColor; } }
    public Color DebugLegColor           { get { return _debugLegColor; } }
    public Color DebugTargetColor        { get { return _debugTargetColor; } }
    public Color DebugLegWireframeColor  { get { return _debugLegWireframeColor; } }

    public float DebugBonePositionRadius { get { return _debugBonePositionRadius; } }
    public float DebugTargetRadius       { get { return _debugTargetRadius; } }
    public float DebugPoleRadius         { get { return _debugPoleRadius; } }
    public float DebugSegmentWidth       { get { return _debugSegmentWidth; } }
}
