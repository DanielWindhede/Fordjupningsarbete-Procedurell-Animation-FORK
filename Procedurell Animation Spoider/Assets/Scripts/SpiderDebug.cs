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
    [SerializeField] private bool _showAverageLegPosition = true;
    [SerializeField] private bool _showDistanceToTarget = true;
    [SerializeField] private bool _showAverageLegPositionSphere = true;
    [SerializeField] private bool _showDistanceFromAverageLegPosition = true;

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
    [SerializeField] private Color _averageLegPositionColor;
    [SerializeField] private Color _averageLegPositionSphereColor;
    [SerializeField] private Color _distanceFromAverageLegPositionColor;

    [SerializeField, Range(0.01f, 10f)] private float _jointPositionRadius;
    [SerializeField, Range(0.01f, 10f)] private float _targetRadius;
    [SerializeField, Range(0.01f, 10f)] private float _legTargetPositionRadius;
    [SerializeField, Range(0.01f, 10f)] private float _poleRadius;
    [SerializeField, Range(0.01f, 10f)] private float _virtualLegTargetRadiusRadius = 0.1f;
    [SerializeField, Range(0.01f, 10f)] private float _segmentWidth;
    [SerializeField, Range(0.01f, 10f)] private float _averageLegPositionRadius;

    public bool ShowJoints                           { get { return _showJoints; } }
    public bool ShowLegsWireframes                   { get { return _showLegsWireframes; } }
    public bool ShowLegs                             { get { return _showLegs; } }
    public bool ShowTargets                          { get { return _showTargets; } }
    public bool ShowPoles                            { get { return _showPoles; } }
    public bool ShowLegTargets                       { get { return _showLegTargets; } }
    public bool ShowVirtualLegTarget                 { get { return _showPoles; } }
    public bool ShowAverageLegPosition               { get { return _showAverageLegPosition; } }
    public bool ShowDistanceToTarget                 { get { return _showDistanceToTarget; } }
    public bool ShowAverageLegPositionSphere         { get { return _showAverageLegPositionSphere; } }
    public bool ShowDistanceFromAverageLegPosition   { get { return _showDistanceFromAverageLegPosition; } }
                                                     
    public Color JointColor                          { get { return _jointColor; } }
    public Color PoleColor                           { get { return _poleColor; } }
    public Color LegColor                            { get { return _legColor; } }
    public Color TargetColor                         { get { return _targetColor; } }
    public Color LegWireframeColor                   { get { return _LegWireframeColor; } }
    public Color LegTargetColor                      { get { return _legTargetColor; } }
    public Color VirtualLegTargetRadiusColor         { get { return _virtualLegTargetRadiusColor; } }
    public Color VirtualLegTargetColor               { get { return _virtualLegTargetColor; } }
    public Color DistanceColor                       { get { return _distanceColor; } }
    public Color AverageLegPositionColor             { get { return _averageLegPositionColor; } }
    public Color DistanceFromAverageLegPositionColor { get { return _distanceFromAverageLegPositionColor; } }
    public Color AverageLegPositionSphereColor       { get { return _averageLegPositionSphereColor; } }
                                                   
    public float JointPositionRadius                 { get { return _jointPositionRadius; } }
    public float TargetRadius                        { get { return _targetRadius; } }
    public float LegTargetPositionRadius             { get { return _legTargetPositionRadius; } }
    public float PoleRadius                          { get { return _poleRadius; } }
    public float VirtualLegTargetRadiusRadius        { get { return _virtualLegTargetRadiusRadius; } }
    public float SegmentWidth                        { get { return _segmentWidth; } }
    public float AverageLegPositionRadius            { get { return _averageLegPositionRadius; } }
}
