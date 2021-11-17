/**********************************************************************************************************************************************************
 * XRUX_Editor_Settings
 * --------------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for the various objects
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Common Functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public static class XRUX_Editor_Settings
{
    public static GUIStyle headingStyle
    {
        get {
            GUIStyle headingStyle = new GUIStyle();
            headingStyle.fontStyle = FontStyle.Normal;
            headingStyle.fontSize = 18;
            headingStyle.alignment = TextAnchor.MiddleCenter;
            headingStyle.normal.textColor = Color.white;
            headingStyle.wordWrap = true;

            return headingStyle;
        }
    }


    public static GUIStyle descriptionStyle
    {
        get {
            GUIStyle descriptionStyle = new GUIStyle();
            descriptionStyle.fontStyle = FontStyle.Italic;
            descriptionStyle.fontSize = 13;
            descriptionStyle.alignment = TextAnchor.MiddleCenter;
            descriptionStyle.normal.textColor = Color.grey;
            descriptionStyle.wordWrap = true;

            return descriptionStyle;
        }
    }
    public static GUIStyle subHeadingStyle
    {    
        get {
            GUIStyle subHeadingStyle = new GUIStyle();
            subHeadingStyle.fontStyle = FontStyle.Normal;
            subHeadingStyle.fontSize = 13;
            subHeadingStyle.alignment = TextAnchor.MiddleLeft;
            subHeadingStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            return subHeadingStyle;
        }
    }

    public static GUIStyle fieldStyle
    {    
        get {
            GUIStyle fieldstyle = new GUIStyle();
            fieldstyle.fontStyle = FontStyle.Normal;
            fieldstyle.fontSize = 12;
            fieldstyle.alignment = TextAnchor.MiddleLeft;
            fieldstyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            return fieldstyle;
        }
    }


    public static GUIStyle categoryStyle
    {    
        get {
            GUIStyle categoryStyle = new GUIStyle();
            categoryStyle.fontStyle = FontStyle.Italic;
            categoryStyle.fontSize = 14;
            categoryStyle.alignment = TextAnchor.MiddleLeft;
            categoryStyle.normal.textColor = Color.grey;

            return categoryStyle;
        }
    }


    public static GUIStyle helpTextStyle
    {    
        get {
            GUIStyle helpTextStyle = new GUIStyle();
            helpTextStyle.fontStyle = FontStyle.Italic;
            helpTextStyle.fontSize = 12;
            helpTextStyle.alignment = TextAnchor.MiddleLeft;
            helpTextStyle.normal.textColor = Color.grey;
            helpTextStyle.wordWrap = true;

            return helpTextStyle;
        }
    }


    public static void DrawSetupHeading() { XRUX_Editor_Settings.DrawSubHeading("SETUP"); }
    public static void DrawInputsHeading() { XRUX_Editor_Settings.DrawSubHeading("INPUTS"); }
    public static void DrawParametersHeading() { XRUX_Editor_Settings.DrawSubHeading("PARAMETERS"); }
    public static void DrawOutputsHeading() { XRUX_Editor_Settings.DrawSubHeading("OUTPUTS"); }

    public static void DrawUILine(Color color)
    {
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = color;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }



    public static void DrawMainHeading(string title, string description = "")
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, XRUX_Editor_Settings.headingStyle);
        EditorGUILayout.Space();
        if (description != "")
        {
            EditorGUILayout.LabelField(description, XRUX_Editor_Settings.descriptionStyle);
        }
    }

    public static void DrawSubHeading(string title)
    {
        XRUX_Editor_Settings.DrawUILine(Color.grey);
        EditorGUILayout.LabelField(title, XRUX_Editor_Settings.subHeadingStyle);
        XRUX_Editor_Settings.DrawUILine(Color.grey);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
