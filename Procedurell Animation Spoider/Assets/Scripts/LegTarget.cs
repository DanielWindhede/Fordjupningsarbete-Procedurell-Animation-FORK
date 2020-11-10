using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    public Vector3 Position { get { return this.transform.position; } set { this.transform.position = value; } }
}
