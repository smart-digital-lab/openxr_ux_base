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
using JSONEncoderDecoder;

public enum OpenXR_UX_Layers { OpenXR_UX = 6, Go_Areas, NoGo_Areas, Player, Other_Players }
public enum OpenXR_UX_Tags { XREvents, XRLeft, XRRight, XRMuxEvents }

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData that is used as events that are sent between XR Modules
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRData
{
    public enum XRDataType { INT, FLOAT, BOOL, STRING, VECTOR3 }
    public enum Mode { User, Advanced }

    private XRDataType theType;
    private int intValue = 0;
    private float floatValue = 0.0f;
    private bool boolValue = false;
    private string stringValue = "";
    private Vector3 vector3Value = new Vector3();
    private bool quietlyValue = false;

    public bool quietly { get { return quietlyValue; }  set { quietlyValue = value; } }

    public new XRDataType GetType() { return theType; }
    public int ToInt() { return intValue; }
    public float ToFloat() { return floatValue; }
    public bool ToBool() { return boolValue; }
    public override string ToString() {return stringValue; }
    public Vector3 ToVector3() {return vector3Value; }

    public XRData(float newValue, bool newQuietly = false)
    {
        intValue = Mathf.RoundToInt(newValue);
        floatValue = newValue;
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        vector3Value.x = newValue;
        quietly = newQuietly;
        theType = XRDataType.FLOAT;
    }
    public XRData(int newValue, bool newQuietly = false)
    {
        intValue = newValue;
        floatValue = Convert.ToSingle(newValue);
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        vector3Value.x = Convert.ToSingle(newValue);
        quietly = newQuietly;
        theType = XRDataType.INT;
    }
    public XRData(bool newValue, bool newQuietly = false)
    {
        intValue = Convert.ToInt32(newValue);
        floatValue = Convert.ToSingle(newValue);
        boolValue = newValue;
        stringValue = newValue.ToString();
        vector3Value.x = Convert.ToSingle(newValue);
        quietly = newQuietly;
        theType = XRDataType.BOOL;
    }
    public XRData(string newValue, bool newQuietly = false)
    {
        int.TryParse(newValue, out intValue);
        float.TryParse(newValue, out floatValue);
        bool.TryParse(newValue, out boolValue);
        stringValue = newValue;
        vector3Value = ToVector3(newValue);
        quietly = newQuietly;
        theType = XRDataType.STRING;
    }

    public XRData(Vector3 newValue, bool newQuietly = false)
    {
        intValue = Mathf.RoundToInt(newValue.x);
        floatValue = newValue.x;
        boolValue = Convert.ToBoolean(newValue.x);
        stringValue = FromVector3(newValue);
        vector3Value = newValue;
        quietly = newQuietly;
        theType = XRDataType.FLOAT;
    }

    public static Vector3 ToVector3(string data)
    {
        ArrayList arrayData = (ArrayList) JSON.JsonDecode(data);
        return ToVector3(arrayData);
    }
    public static Vector3 ToVector3(ArrayList data)
    {
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        if (data != null)
        {
            float.TryParse(data[0].ToString(), out x);
            float.TryParse(data[1].ToString(), out y);
            float.TryParse(data[2].ToString(), out z);
        }
        return new Vector3(x,y,z);
    }
    public static string FromVector3(Vector3 data)
    {
        return ("[" + data.x + "," + data.y + "," + data.z + "]");
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
[Serializable]
public class UnityVector3Event : UnityEvent<Vector3> {}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------