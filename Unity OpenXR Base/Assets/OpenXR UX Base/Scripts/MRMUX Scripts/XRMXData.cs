/**********************************************************************************************************************************************************
 * XRMXData
 * --------
 *
 * 2021-11-08
 *
 * The XRMX data structure transmits info from the websockets to the object that needs it, and vice versa.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JSONEncoderDecoder;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRMXData is used to synchronize object data between devices
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRMXData
{
    public enum XRMXDataType { INT, FLOAT, BOOL, STRING, VECTOR3 }
    public enum XRMXDataDirection { IN, OUT }

    public string objectName; // The name of the object this data is about
    public string objectParameter; // The parameter on the object this data is for / from
    public XRMXDataDirection direction; // Going out or coming in...

    private XRMXDataType theType; // The data type being transmitted
    private int intValue = 0;
    private float floatValue = 0.0f;
    private bool boolValue = false;
    private string stringValue = "";
    private Vector3 vector3Value = new Vector3();


    public new XRMXDataType GetType() { return theType; }
    public int ToInt() { return intValue; }
    public float ToFloat() { return floatValue; }
    public bool ToBool() { return boolValue; }
    public override string ToString() { return stringValue; }

    public XRMXData(ArrayList newData, XRMXDataDirection newDirection)
    {
        // Data should be a JSON array with 5 items [appname, objectname, parameter, type, value]
        direction = newDirection;
        objectName = (string) newData[1];
        objectParameter = (string) newData[2];
        string objectType = (string) newData[3];

        switch (objectType)
        {
            case "int":
                int.TryParse(newData[4].ToString(), out intValue);
                floatValue = Convert.ToSingle(intValue);
                boolValue = Convert.ToBoolean(intValue);
                stringValue = intValue.ToString();
                theType = XRMXDataType.INT;
                break;

            case "float":
                float.TryParse(newData[4].ToString(), out floatValue);
                intValue = Mathf.RoundToInt(floatValue);
                boolValue = Convert.ToBoolean(floatValue);
                stringValue = floatValue.ToString();
                theType = XRMXDataType.FLOAT;
                break;

            case "bool":
                bool.TryParse(newData[4].ToString(), out boolValue);
                intValue = Convert.ToInt32(boolValue);
                floatValue = Convert.ToSingle(boolValue);
                stringValue = boolValue.ToString();
                theType = XRMXDataType.BOOL;
                break;

            case "vector3":
                vector3Value = ConvertToVector3((ArrayList) newData[4]);
                stringValue = vector3Value.ToString();
                theType = XRMXDataType.VECTOR3;
                break;

            default:
                stringValue = (string) newData[4];
                theType = XRMXDataType.STRING;
            break;
        }
    }

    public XRMXData(string newObjectname, string newParameter, float newValue, XRMXDataDirection newDirection)
    {
        direction = newDirection;
        objectName = newObjectname;
        objectParameter = newParameter;
        intValue = Mathf.RoundToInt(newValue);
        floatValue = newValue;
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        theType = XRMXDataType.FLOAT;
    }
    public XRMXData(string newObjectname, string newParameter, int newValue, XRMXDataDirection newDirection)
    {
        direction = newDirection;
        objectName = newObjectname;
        objectParameter = newParameter;
        intValue = newValue;
        floatValue = Convert.ToSingle(newValue);
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        theType = XRMXDataType.INT;
    }
    public XRMXData(string newObjectname, string newParameter, bool newValue, XRMXDataDirection newDirection)
    {
        direction = newDirection;
        objectName = newObjectname;
        objectParameter = newParameter;
        intValue = Convert.ToInt32(newValue);
        floatValue = Convert.ToSingle(newValue);
        boolValue = newValue;
        stringValue = newValue.ToString();
        theType = XRMXDataType.BOOL;
    }
    public XRMXData(string newObjectname, string newParameter, string newValue, XRMXDataDirection newDirection)
    {
        direction = newDirection;
        objectName = newObjectname;
        objectParameter = newParameter;
        int.TryParse(newValue, out intValue);
        float.TryParse(newValue, out floatValue);
        bool.TryParse(newValue, out boolValue);
        vector3Value = ConvertToVector3(newValue);
        stringValue = newValue;
        theType = XRMXDataType.STRING;
    }

    public XRMXData(string newObjectname, string newParameter, Vector3 newValue, XRMXDataDirection newDirection)
    {
        direction = newDirection;
        objectName = newObjectname;
        objectParameter = newParameter;
        stringValue = newValue.ToString();
        vector3Value = newValue;
        theType = XRMXDataType.VECTOR3;
    }


    Vector3 ConvertToVector3(string data)
    {
        ArrayList arrayData = (ArrayList) JSON.JsonDecode(data);
        return ConvertToVector3(arrayData);
    }
    Vector3 ConvertToVector3(ArrayList data)
    {
        float x, y, z = 0.0f;
        float.TryParse(data[0].ToString(), out x);
        float.TryParse(data[1].ToString(), out y);
        float.TryParse(data[2].ToString(), out z);
        return new Vector3(x,y,z);
    }
}
// ------------------------------------------------------------------------------------------------







// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The type of data that can be sent via the XR Event
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRMXEvent
{
    public XRMXData data;
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
