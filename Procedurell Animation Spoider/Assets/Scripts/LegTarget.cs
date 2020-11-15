using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Color _rayCastColor;
    [SerializeField, Range(0.01f, 100)] private float _maxDistance = 1.5f;
    [SerializeField, Range(-20f, 20)] private float _shootRayYPosition = -0.15f;

    private ActorControl _actorControl;
    private Vector3 _virtualDirection;

    public Vector3 Position { get { return transform.position; } private set { transform.position = value; } }
    public Vector3 VirtualPosition { get; private set; }
    public Vector3 Normal { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    private void OnValidate()
    {
        Initialize();
    }

    private void Initialize()
    {
        _actorControl = GetComponentInParent<ActorControl>();
    }

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (_actorControl.ControlSpider.Direction != Vector3.zero)
            _virtualDirection = _actorControl.ControlSpider.Direction.normalized;

        Vector3 origin = transform.position + _virtualDirection * _actorControl.MoveSpiderLegs.VirtualLegTargetRadius;
        origin += _body.up * _shootRayYPosition;

        Debug.DrawLine(origin, origin - _body.up * _maxDistance, _rayCastColor);

        RaycastHit hit;
        if(Physics.Raycast(origin, -_body.up, out hit, _maxDistance, _layerMask))
        {
            Normal = hit.normal;
            VirtualPosition = hit.point;
            transform.position = hit.point - _virtualDirection.normalized * _actorControl.MoveSpiderLegs.VirtualLegTargetRadius;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_actorControl.SpiderDebug != null)
        {
            if (_actorControl.SpiderDebug.ShowVirtualLegTarget)
            {
                Gizmos.color = _actorControl.SpiderDebug.VirtualLegTargetColor;
                Gizmos.DrawSphere(VirtualPosition, _actorControl.SpiderDebug.VirtualLegTargetRadiusRadius);

                Handles.color = _actorControl.SpiderDebug.VirtualLegTargetRadiusColor;
                Handles.DrawWireDisc(transform.position, _body.up, _actorControl.MoveSpiderLegs.VirtualLegTargetRadius);
            }
            
            if (_actorControl.SpiderDebug.ShowLegTargets)
            {
                Gizmos.color = _actorControl.SpiderDebug.LegTargetColor;
                Gizmos.DrawSphere(Position, _actorControl.SpiderDebug.LegTargetPositionRadius);
            }
        }
    }
}