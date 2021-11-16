/**********************************************************************************************************************************************************
 * XRData_Calc
 * -----------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_Calc
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Calc
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Calc))]
public class XRData_Calc_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Calc myTarget = (XRData_Calc)target;

        XRUX_Editor_Settings.DrawMainHeading("Calculate", "Calculate using XRData. Each time one of the inputs changes, the calculation is performed and an XRData event generated.  The order is: InputA operation InputB.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("InputA", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputB", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.op = (XRData_Calc.operation) EditorGUILayout.EnumPopup("Operation to perform", myTarget.op);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
