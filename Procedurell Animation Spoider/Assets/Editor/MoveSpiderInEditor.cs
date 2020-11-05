using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpiderDebug)), CanEditMultipleObjects]
public class MoveSpiderInEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        SpiderDebug me = (SpiderDebug)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(me.CurrentMoveSpiderAxisPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(me, "Change Position");
            me.CurrentMoveSpiderAxisPosition = newTargetPosition;
            me.MoveSpider(me.CurrentMoveSpiderAxisPosition);
        }
    }
}
