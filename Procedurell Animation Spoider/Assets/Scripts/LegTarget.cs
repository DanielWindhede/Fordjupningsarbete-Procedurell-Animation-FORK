using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] int _index;

    public Vector3 Position { get { return this.transform.position; } set { this.transform.position = value; } }
    public int Index { get { return _index; } }
}
