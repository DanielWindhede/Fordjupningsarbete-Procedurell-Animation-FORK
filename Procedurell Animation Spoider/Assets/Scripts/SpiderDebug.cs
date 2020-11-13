using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpiderDebug : MonoBehaviour
{
    [Header("Activation")]

    [SerializeField] private bool _showJoints = true;
    [SerializeField] private bool _showLegsWireframes = true;
    [SerializeField] private bool _showLegs = true;
    [SerializeField] private bool _showTargets = true;
    [SerializeField] private bool _showPoles = true;
    [SerializeField] private bool _showLegTargets = true;
    [SerializeField] private bool _showVirtualLegTargetRadius = true;

    [Header("Debug Settings")]

    [SerializeField] private Color _jointColor;
    [SerializeField] private Color _poleColor;
    [SerializeField] private Color _legColor;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Color _LegWireframeColor = Color.white;
    [SerializeField] private Color _legTargetColor;
    [SerializeField] private Color _distanceColor;
    [SerializeField] private Color _virtualLegTargetColor;
    [SerializeField] private Color _virtualLegTargetRadiusColor;

    [SerializeField, Range(0.01f, 10f)] private float _jointPositionRadius;
    [SerializeField, Range(0.01f, 10f)] private float _targetRadius;
    [SerializeField, Range(0.01f, 10f)] private float _legTargetPositionRadius;
    [SerializeField, Range(0.01f, 10f)] private float _poleRadius;
    [SerializeField, Range(0.01f, 10f)] private float _segmentWidth;

    public bool ShowJoints                       { get { return _showJoints; } }
    public bool ShowLegsWireframes               { get { return _showLegsWireframes; } }
    public bool ShowLegs                         { get { return _showLegs; } }
    public bool ShowTargets                      { get { return _showTargets; } }
    public bool ShowPoles                        { get { return _showPoles; } }
    public bool ShowVirtualLegTarget             { get { return _showPoles; } }

    public Color JointColor                      { get { return _jointColor; } }
    public Color PoleColor                       { get { return _poleColor; } }
    public Color LegColor                        { get { return _legColor; } }
    public Color TargetColor                     { get { return _targetColor; } }
    public Color LegWireframeColor               { get { return _LegWireframeColor; } }
    public Color LegTargetColor                  { get { return _legTargetColor; } }
    public Color VirtualLegTargetRadiusColor     { get { return _virtualLegTargetRadiusColor; } }
    public Color VirtualLegTargetColor           { get { return _virtualLegTargetColor; } }
    public Color DistanceColor                   { get { return _distanceColor; } }
    
    public float JointPositionRadius             { get { return _jointPositionRadius; } }
    public float TargetRadius                    { get { return _targetRadius; } }
    public float LegTargetPositionRadius         { get { return _legTargetPositionRadius; } }
    public float PoleRadius                      { get { return _poleRadius; } }
    public float SegmentWidth                    { get { return _segmentWidth; } }
}
