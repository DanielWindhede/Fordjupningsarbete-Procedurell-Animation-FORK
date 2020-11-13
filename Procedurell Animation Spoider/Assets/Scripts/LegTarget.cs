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

    private SpiderDebug _spiderDebug;
    private ControlSpider _controlSpider;
    private MoveSpiderLegs _moveSpiderLegs;
    private Vector3 _virtualDirection;
    private Vector3 _virtualPosition;

    public Vector3 Position { get { return transform.position; } private set { transform.position = value; } }
    public Vector3 VirtualPosition { get { return _virtualPosition; } private set { _virtualPosition = value; } }
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
        _spiderDebug = GetComponentInParent<SpiderDebug>();
        _controlSpider = GetComponentInParent<ControlSpider>();
        _moveSpiderLegs = GetComponentInParent<MoveSpiderLegs>();
        _virtualPosition = transform.position;
    }

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (_controlSpider.Direction != Vector3.zero)
            _virtualDirection = _controlSpider.Direction.normalized;

        Vector3 origin = transform.position + _virtualDirection * _moveSpiderLegs.VirtualLegTargetRadius;
        origin += _body.up * _shootRayYPosition;

        Debug.DrawLine(origin, origin - _body.up * _maxDistance, _rayCastColor);

        RaycastHit hit;
        if(Physics.Raycast(origin, -_body.up, out hit, _maxDistance, _layerMask))
        {
            Normal = hit.normal;
            _virtualPosition = hit.point;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_spiderDebug.ShowVirtualLegTarget)
        {
            Gizmos.color = _spiderDebug.VirtualLegTargetColor;
            Gizmos.DrawSphere(_virtualPosition, 0.1f);

            Handles.color = _spiderDebug.VirtualLegTargetRadiusColor;
            Handles.DrawWireDisc(transform.position, _body.up, _moveSpiderLegs.VirtualLegTargetRadius);
        }
    }
}