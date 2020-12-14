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

    private void SetClayObjectParameters(ClayObject clayObject, SandBlobData data, Vector3 position) {

        clayObject.transform.localScale = data.size;
        clayObject.color = data.color;
        clayObject.blend = data.blend / 100;
        clayObject.attrs = new Vector4(data.round / 100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(data.primitiveShape);
        clayObject.transform.position = new Vector3(position.x, position.y + .4f, position.z); ;
    }

    private void DeleteObjectsInUndoneList() {
        if (undoneClayObjects.Count > 0) {
            foreach (ClayObject clayObject in undoneClayObjects) {
                Destroy(clayObject.gameObject);
            }
            undoneClayObjects.Clear();
        }
    }

    public void Initialize() {
        ToggleSolids(true);
        baseObject.enabled = false;
    }

    public Transform AddClayObject(SandBlobData data, Vector3 spawnPosition) {
        
        ClayObject clayObject = clayContainer.addClayObject();
        SetClayObjectParameters(clayObject, data, spawnPosition);
        clayObject.enabled = false;
        allClayObjects.Add(clayObject);
        DeleteObjectsInUndoneList();

        return clayObject.transform;
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


    public Vector3 Undo() {

        if (allClayObjects.Count > 0) {
            ClayObject lastClayObject = allClayObjects[allClayObjects.Count - 1];
            undoneClayObjects.Add(lastClayObject);
            allClayObjects.Remove(lastClayObject);
            lastClayObject.enabled = true;
            lastClayObject.transform.hasChanged = true;
            lastClayObject.gameObject.SetActive(false);
            lastClayObject.enabled = false;
            UpdateSolids();

            return lastClayObject.transform.position;
        }
        return Vector3.zero;
    }

    public Vector3 Redo() {

        if (undoneClayObjects.Count > 0) {
            ClayObject lastRemovedClayObject = undoneClayObjects[undoneClayObjects.Count - 1];
            undoneClayObjects.Remove(lastRemovedClayObject);
            allClayObjects.Add(lastRemovedClayObject);
            lastRemovedClayObject.enabled = true;
            lastRemovedClayObject.transform.hasChanged = true;
            lastRemovedClayObject.gameObject.SetActive(true);
            lastRemovedClayObject.enabled = false;
            UpdateSolids();

            return lastRemovedClayObject.transform.position;
        }
        return Vector3.zero;
    }


}
