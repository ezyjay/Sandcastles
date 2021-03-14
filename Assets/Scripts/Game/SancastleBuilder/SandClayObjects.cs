using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clayxels;

/// <summary>
/// Contains all logic to do with Clayxels within the sandcastle building system
/// </summary>
public class SandClayObjects : MonoBehaviour
{
    public ClayContainer clayContainer;

    private ClayObject baseObject;
    private List<ClayObject> allClayObjects = new List<ClayObject>();
    private List<ClayObject> undoneClayObjects = new List<ClayObject>();

    private void Awake() {
        baseObject = clayContainer.GetComponentInChildren<ClayObject>();
    }

    private void SetClayObjectParameters(ClayObject clayObject, SandBlobData data, Vector3 position, OperationType operationType = OperationType.ADD) {

        clayObject.transform.localScale = data.size;
        clayObject.color = data.color;

        clayObject.blend = data.blend / 100;
        if (operationType == OperationType.SUBTRACT)
            clayObject.blend *= -1;

        //Cube or cylinder
        if (data.primitiveShape == 0 || data.primitiveShape == 2) {
            clayObject.transform.rotation = data.rotation;
            float zValue = data.primitiveShape == 2 ? data.cone / 100 : clayObject.attrs.z;
            clayObject.attrs = new Vector4(data.round / 100, clayObject.attrs.y, zValue, clayObject.attrs.w);
            //Torus
        } else if (data.primitiveShape == 3)
            clayObject.attrs = new Vector4(data.fat / 100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);


        clayObject.setPrimitiveType(data.primitiveShape);
        clayObject.transform.position = new Vector3(position.x, position.y + .4f, position.z); ;
    }

    private void DeleteObjectsInUndoneList() {
        if (undoneClayObjects.Count > 0) {
            foreach (ClayObject clayObject in undoneClayObjects) {
                if (clayObject != null)
                    Destroy(clayObject.gameObject);
            }
            undoneClayObjects.Clear();
        }
    }

    private void CommitClayObject(ClayObject clayObject) {
        clayObject.enabled = false;
        allClayObjects.Add(clayObject);
        DeleteObjectsInUndoneList();
    }

    public void Initialize() {
        ToggleSolids(true);
        baseObject.enabled = false;
    }

    public Transform AddClayObject(SandBlobData data, Vector3 spawnPosition, OperationType operationType = OperationType.ADD, bool temporaryAdd = false) {
        
        ClayObject clayObject = clayContainer.addClayObject();
        SetClayObjectParameters(clayObject, data, spawnPosition, operationType);

        if (!temporaryAdd) 
            CommitClayObject(clayObject);

        return clayObject.transform;
    }

    public void CommitClayObject(GameObject tempClayObject) {
        ClayObject clayObject = tempClayObject.GetComponent<ClayObject>();
        CommitClayObject(clayObject);
    }

    public void UpdateSolids() {
        clayContainer.forceUpdateAllSolids();
        ToggleSolids(true);
    }

    public void ToggleSolids(bool on) {

        if (baseObject == null)
            baseObject = clayContainer.GetComponentInChildren<ClayObject>();

        baseObject.gameObject.SetActive(true);
        foreach (ClayObject clayObject in allClayObjects) {
            clayObject.gameObject.SetActive(true);
        }
    }

    public void DestroyAllSolids(bool includeBase = false) {
        int i = 0;
        foreach (Transform clayObject in clayContainer.transform) {
            //Don't destroy first child because it's the base
            if (includeBase || i > 0) {
                Destroy(clayObject.gameObject);
            }
            i++;
        }
        allClayObjects.Clear();
    }


    public Vector3 Undo(out bool lastObjectWasSubstract) {

        lastObjectWasSubstract = false;

        if (allClayObjects.Count > 0) {
            ClayObject lastClayObject = allClayObjects[allClayObjects.Count - 1];
            undoneClayObjects.Add(lastClayObject);
            allClayObjects.Remove(lastClayObject);
            lastClayObject.enabled = true;
            lastClayObject.transform.hasChanged = true;
            lastClayObject.gameObject.SetActive(false);
            lastClayObject.enabled = false;

            if (lastClayObject.blend < 0)
                lastObjectWasSubstract = true;

            UpdateSolids();

            return lastClayObject.transform.position;
        }
        return Vector3.zero;
    }

    public Vector3 Redo(out bool lastRemovedObjectWasSubstract) {


        lastRemovedObjectWasSubstract = false;

        if (undoneClayObjects.Count > 0) {

            ClayObject lastRemovedClayObject = undoneClayObjects[undoneClayObjects.Count - 1];
            undoneClayObjects.Remove(lastRemovedClayObject);
            allClayObjects.Add(lastRemovedClayObject);
            lastRemovedClayObject.enabled = true;
            lastRemovedClayObject.transform.hasChanged = true;
            lastRemovedClayObject.gameObject.SetActive(true);
            lastRemovedClayObject.enabled = false;

            if (lastRemovedClayObject.blend < 0)
                lastRemovedObjectWasSubstract = true;

            UpdateSolids();

            return lastRemovedClayObject.transform.position;
        }
        return Vector3.zero;
    }


}
