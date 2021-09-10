using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameObjectMenus : MonoBehaviour
{
    private static void CreateObjectFromPrefab(string Location, string Name)
    {
        GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(Location));
        prefab.name = Name;

        if(Selection.activeTransform != null)
        {
            prefab.transform.SetParent(Selection.activeTransform, false);
        }
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localEulerAngles = Vector3.zero;
        prefab.transform.localScale = Vector3.one;

        Selection.activeGameObject = prefab;
    }

    [MenuItem ("GameObject/XR/UX/XR Buttons/XR Button")]
    private static void CreateXRButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Button.prefab", "XR Button");
    }

    [MenuItem ("GameObject/XR/UX/XR Buttons/XR Cancel Button")]
    private static void CreateXRCancelButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Cancel Button.prefab", "XR Cancel Button");
    }

    [MenuItem ("GameObject/XR/UX/XR Buttons/XR OK Button")]
    private static void CreateXROKButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR OK Button.prefab", "XR OK Button");
    }

    [MenuItem ("GameObject/XR/UX/XR Buttons/XR Square Button")]
    private static void CreateXRSquareButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Square Button.prefab", "XR Square Button");
    }

    [MenuItem ("GameObject/XR/UX/XR Console")]
    private static void CreateXRConsole ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Console.prefab", "XR Console");
    }

    [MenuItem ("GameObject/XR/UX/XR Group")]
    private static void CreateXRGroup ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Group.prefab", "XR Group");
    }

    [MenuItem ("GameObject/XR/UX/XR Inputfield")]
    private static void CreateXRInputfield ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Inputfield.prefab", "XR Inputfield");
    }

    [MenuItem ("GameObject/XR/UX/XR Keyboard")]
    private static void CreateXRKeyboard ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Keyboard.prefab", "XR Keyboard");
    }

    [MenuItem ("GameObject/XR/UX/XR Knob")]
    private static void CreateXRKnob ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Knob.prefab", "XR Knob");
    }

    [MenuItem ("GameObject/XR/UX/XR Radio Button")]
    private static void CreateXRRadioButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Radio Button.prefab", "XR Radio Button");
    }

    [MenuItem ("GameObject/XR/UX/XR Radio Buttons")]
    private static void CreateXRRadioButtons ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Radio Buttons.prefab", "XR Radio Buttons");
    }

    [MenuItem ("GameObject/XR/UX/XR Slider Switch")]
    private static void CreateXRSliderSwitch ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Slider Switch.prefab", "XR Slider Switch");
    }

    [MenuItem ("GameObject/XR/UX/XR Textfield")]
    private static void CreateXRTextfield ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Textfield.prefab", "XR Textfield");
    }

    [MenuItem ("GameObject/XR/UX/XR Toggle Button")]
    private static void CreateXRToggleButton ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR Toggle Button.prefab", "XR Toggle Button");
    }

    [MenuItem ("GameObject/XR/UX/XR UI Base")]
    private static void CreateXRUIBase ()
    {
        CreateObjectFromPrefab("Assets/Prefabs/XR UI Base.prefab", "XR UI Base");
    }

    [MenuItem ("GameObject/XR/Convert Main Camera To XR Rig With UX")]
    private static void ConvertMainCameraToXRRigWithUX ()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            if (mainCamera.gameObject.transform.root == mainCamera.gameObject.transform)
            {
                DestroyImmediate(mainCamera.gameObject);
                CreateObjectFromPrefab("Assets/Prefabs/XRRig with UX.prefab", "XRRig with UX");
            }
        }
    }    
}
