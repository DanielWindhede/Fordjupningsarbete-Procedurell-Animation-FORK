using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorControl : MonoBehaviour
{
    public ControlSpider ControlSpider { get; private set; }
    public SpiderDebug SpiderDebug { get; private set; }
    public MoveSpiderLegs MoveSpiderLegs { get; private set; }

    private void Awake()
    {
        ControlSpider = GetComponent<ControlSpider>();
        SpiderDebug = GetComponent<SpiderDebug>();
        MoveSpiderLegs = GetComponent<MoveSpiderLegs>();
    }
}
