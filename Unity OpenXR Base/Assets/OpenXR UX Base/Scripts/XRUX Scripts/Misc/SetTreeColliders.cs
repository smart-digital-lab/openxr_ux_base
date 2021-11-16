/**********************************************************************************************************************************************************
 * SetTreeColliders
 * ----------------
 *
 * 2021-09-20
 * 
 * Replaces the colliders on each tree with ones that works with the navigation system.  This is required as otherwise the colliders that the trees have
 * are in the wrong layer to be noticed by our raycasting movement algorithm.
 * 
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class SetTreeColliders : MonoBehaviour
{
    [Header("The size of the terrain in world units from one side to the other.")]
    public float terrainScale = 1000.0f;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Do these alterations on starting the VE
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        Terrain theTerrain = Terrain.activeTerrain;
        TreeInstance[] theTrees = theTerrain.terrainData.treeInstances;

        foreach (TreeInstance theTree in theTrees)
        {
            TreePrototype treePrototype = theTerrain.terrainData.treePrototypes[theTree.prototypeIndex];
            Collider[] theColliders = treePrototype.prefab.gameObject.GetComponentsInChildren<Collider>();
            if (theColliders.Length != 0)
            {
                // Create a new Parent
                GameObject theParent = new GameObject(treePrototype.prefab.gameObject.name + "_colliders");

                // Place all the colliders in the world where the tree is
                Vector3 worldPosition = new Vector3(theTree.position.x * terrainScale + theTerrain.gameObject.transform.position.x, 0.0f, theTree.position.z * terrainScale + theTerrain.gameObject.transform.position.z);

                // Get the height of the terrain at that point because theTree.position is only the x,z values
                float terrainHeight = theTerrain.SampleHeight(worldPosition) - theTerrain.transform.position.y;

                // Duplicate all the colliders
                foreach (Collider theCollider in theColliders)
                {
                    // Duplicate the collider
                    Collider newCollider = Instantiate(theCollider);

                    // Put it on the collision layer
                    newCollider.gameObject.layer = 8;

                    // Parent it to the main gameobject
                    newCollider.transform.SetParent(theParent.transform);

                    // Set the collider's position
                    newCollider.transform.localPosition = new Vector3(theCollider.transform.position.x, theCollider.transform.position.y, theCollider.transform.position.z);
                }

                // Put the colliders as children to the terrain
                theParent.transform.SetParent(theTerrain.transform);

                // Set the colliders' position, rotation and scale to match the tree instance.
                theParent.transform.localPosition = new Vector3(worldPosition.x - theTerrain.gameObject.transform.position.x, terrainHeight + theTerrain.gameObject.transform.position.y, worldPosition.z - theTerrain.gameObject.transform.position.z);
                theParent.transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * theTree.rotation, 0.0f);
                theParent.transform.localScale = new Vector3(theTree.widthScale, theTree.heightScale, theTree.widthScale);
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------