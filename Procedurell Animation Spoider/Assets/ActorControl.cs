using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorControl : MonoBehaviour
{
    public ControlSpider ControlSpider { get; private set; }
    public SpiderDebug SpiderDebug { get; private set; }
    public MoveSpiderLegs MoveSpiderLegs { get; private set; }

    private FixJoints _fixJoints;
    private LegTarget[] _legTargets;
    private InverseKinematics[] _inverseKinematics;

    private void Awake()
    {
        ControlSpider = GetComponent<ControlSpider>();
        SpiderDebug = GetComponent<SpiderDebug>();
        MoveSpiderLegs = GetComponent<MoveSpiderLegs>();
        _fixJoints = GetComponentInChildren<FixJoints>();
        _legTargets = GetComponentsInChildren<LegTarget>();
        _inverseKinematics = GetComponentsInChildren<InverseKinematics>();
    }

    private void FixedUpdate()
    {
        MoveSpiderLegs.DoFixedUpdate();
        ControlSpider.DoFixedUpdate();
        _fixJoints.DoFixedUpdate();

        for (int i = 0; i < _legTargets.Length; i++)
            _legTargets[i].DoFixedUpdate();

        for (int i = 0; i < _inverseKinematics.Length; i++)
            _inverseKinematics[i].DoInverseKinematics();
    }
}
