/**********************************************************************************************************************************************************
 * XRData
 * ------
 *
 * 2021-10-06
 *
 * The XRData data structure used between XRUX Modules.  When an event is sent between modules, it contains also data.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData that is used as events that are sent between XR Modules
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRData
{
    public enum XRDataType { INT, FLOAT, BOOL, STRING }

    private XRDataType theType;
    private int intValue = 0;
    private float floatValue = 0.0f;
    private bool boolValue = false;
    private string stringValue = "";
    private bool quietlyValue = false;

    public bool quietly { get { return quietlyValue; }  set { quietlyValue = value; } }

    public new XRDataType GetType() { return theType; }
    public int ToInt() { return intValue; }
    public float ToFloat() { return floatValue; }
    public bool ToBool() { return boolValue; }
    public override string ToString() {return stringValue; }

    public XRData(float newValue, bool newQuietly = false)
    {
        intValue = Mathf.RoundToInt(newValue);
        floatValue = newValue;
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.FLOAT;
    }
    public XRData(int newValue, bool newQuietly = false)
    {
        intValue = newValue;
        floatValue = Convert.ToSingle(newValue);
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.INT;
    }
    public XRData(bool newValue, bool newQuietly = false)
    {
        intValue = Convert.ToInt32(newValue);
        floatValue = Convert.ToSingle(newValue);
        boolValue = newValue;
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.BOOL;
    }
    public XRData(string newValue, bool newQuietly = false)
    {
        int.TryParse(newValue, out intValue);
        float.TryParse(newValue, out floatValue);
        bool.TryParse(newValue, out boolValue);
        stringValue = newValue;
        quietly = newQuietly;
        theType = XRDataType.STRING;
    }
}

[Serializable]
public class UnityXRDataEvent : UnityEvent<XRData> {}
[Serializable]
public class UnityIntegerEvent : UnityEvent<int> {}
[Serializable]
public class UnityBooleanEvent : UnityEvent<bool> {}
[Serializable]
public class UnityFloatEvent : UnityEvent<float> {}
[Serializable]
public class UnityStringEvent : UnityEvent<string> {}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------