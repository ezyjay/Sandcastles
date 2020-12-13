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

    private List<ClayObject> allClayObjects = new List<ClayObject>();

    public void Initialize() {
        ToggleSolids(true);
        clayContainer.GetComponentInChildren<ClayObject>().enabled = false;
    }

    public Transform AddClayObject(SandBlobData data, Vector3 spawnPosition) {
        
        ClayObject clayObject = clayContainer.addClayObject();
        SetClayObjectParameters(clayObject, data, spawnPosition);
        clayObject.enabled = false;
        allClayObjects.Add(clayObject);

        return clayObject.transform;
    }

    private void SetClayObjectParameters(ClayObject clayObject, SandBlobData data, Vector3 position) {
        
        clayObject.transform.localScale = data.size;
        clayObject.color = data.color;
        clayObject.blend = data.blend / 100;
        clayObject.attrs = new Vector4(data.round / 100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(data.primitiveShape);
        clayObject.transform.position = new Vector3(position.x, position.y + .4f, position.z); ;
    }

    public void UpdateSolids() {
        clayContainer.forceUpdateAllSolids();
        ToggleSolids(true);
    }

    public void ToggleSolids(bool on) {
        clayContainer.enableAllClayObjects(on);
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
}
