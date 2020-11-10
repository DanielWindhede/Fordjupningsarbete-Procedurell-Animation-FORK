using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSpider : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField, Range(0.01f, 500f)] float _moveSpeed;
    [SerializeField, Range(0.01f, 500f)] float _rotateSpeed;

    [Header("Drop")]

    [SerializeField] Transform _body;

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis(InputManager.HORIZONTAL);
        float vertical = Input.GetAxis(InputManager.VERTICAL);
        float rotate = Input.GetAxis(InputManager.ROTATE);

        Move(horizontal, vertical);
        Rotate(rotate);
    }

    void Move(float horizontal, float vertical)
    {
        Vector3 forward = _body.forward * -vertical;
        Vector3 right = _body.right * -horizontal;
        Vector3 direction = forward + right;
        _body.localPosition = _body.localPosition + direction * Time.fixedDeltaTime * _moveSpeed;
    }

    void Rotate(float rotate)
    {
        Vector3 eulerRotation = _body.localRotation.eulerAngles;
        eulerRotation.y += rotate * _rotateSpeed;
        _body.localRotation = Quaternion.Euler(eulerRotation);
    }
}
