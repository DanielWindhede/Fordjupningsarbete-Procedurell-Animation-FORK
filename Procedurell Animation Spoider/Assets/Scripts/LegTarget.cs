using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] Transform _body;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Color _rayCastColor;
    [SerializeField, Range(0.01f, 100)] float _maxDistance = 1.5f;
    [SerializeField, Range(-20f, 20)] float _shootRayYPosition = -0.15f;
    [SerializeField, Range(-20f, 20f)] float _idealYPosition;

    public Vector3 Position { get { return this.transform.position; } set { this.transform.position = value; } }

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 origin = transform.position;
        origin += _body.up * _shootRayYPosition;

        Debug.DrawLine(origin, origin - _body.up * _maxDistance, _rayCastColor);

        RaycastHit hit;
        if(Physics.Raycast(origin, -_body.up, out hit, _maxDistance, _layerMask))
            transform.position = hit.point;
    }
}
