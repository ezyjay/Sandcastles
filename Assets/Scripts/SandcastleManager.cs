using Clayxels;
using System.Collections.Generic;
using UnityEngine;
using static GridDrawer;

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public ClayContainer clayContainer;
    public GridDrawer gridDrawer;
    public GameObject uiPanel;
    public GameObject sandBlobPreview;
    //public OperationType currentOperationType = OperationType.ADD;
    public SandBlobType currentSandBlobType = SandBlobType.CUBE_1x1;
    public List<SandBlob> possibleSandBlobs = new List<SandBlob>();

    private RaycastHit hit;
    private Ray ray;
    private SandBlob currentSandBlob;

    [EditorButton]
    public void ResetBuildZone() {
        int i = 0;
        foreach (Transform clayObject in clayContainer.transform) {
            //Don't destroy first child because it's the base
            if (i > 0) {
                Destroy(clayObject.gameObject);
            }
            i++;
        }
        gridDrawer.ClearGrid();
    }

    [System.Serializable]
    public struct SandBlob {
        public SandBlobData data;
        public GameObject previewObject;
    }

    [System.Serializable]
    public enum OperationType
    {
        ADD = 0,
        REMOVE = 1,
    }

    private void OnValidate() {
#if UNITY_EDITOR
        ChangeBlobType(currentSandBlobType);
#endif
    }

    private void OnEnable() {
        clayContainer.enableAllClayObjects(true);
        //Disable base clay object so it doesn't update every frame
        clayContainer.GetComponentInChildren<ClayObject>().enabled = false;

        DisableAllPreviews();

        currentSandBlob = possibleSandBlobs.Find(p => p.data.type == currentSandBlobType);
        currentSandBlob.previewObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isInBuildMode) {

            AwaitInput();

            //Ray cast down and if it's a buildable area, spawn sand blob on mouse click
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {

                gridDrawer.MoveObjectOnGrid(currentSandBlob.previewObject.transform, hit.point);
                if (gridDrawer.IsPositionInGrid(currentSandBlob.previewObject.transform.localPosition)) {

                    //Change position of preview object
                    GridIndex gridIndex = gridDrawer.GetTileIndexFromPosition(currentSandBlob.previewObject.transform.position);
                    Vector3 spawnPosition = new Vector3(currentSandBlob.previewObject.transform.position.x, transform.position.y, currentSandBlob.previewObject.transform.position.z);

                    if (gridDrawer.HasObjectInTile(gridIndex)) {
                        float newYPosition = spawnPosition.y + gridDrawer.GetNumberOfObjectsInTile(gridIndex) * gridDrawer.tilesize;
                        spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                        currentSandBlob.previewObject.transform.position = new Vector3(currentSandBlob.previewObject.transform.position.x, spawnPosition.y - .2f * gridDrawer.tilesize, currentSandBlob.previewObject.transform.position.z);
                    }
                    else {
                        currentSandBlob.previewObject.transform.position = new Vector3(currentSandBlob.previewObject.transform.position.x, transform.position.y, currentSandBlob.previewObject.transform.position.z);
                    }

                    currentSandBlob.previewObject.gameObject.SetActive(true);

                    //Add sand blob on click
                    if (Input.GetMouseButtonDown(0)) 
                        AddSandBlob(gridIndex, spawnPosition);
                }
                else {
                    clayContainer.enableAllClayObjects(true);
                    currentSandBlob.previewObject.gameObject.SetActive(false);
                }
            }
            else {
                clayContainer.enableAllClayObjects(true);
                currentSandBlob.previewObject.gameObject.SetActive(false);
            }

        }
    }

    private void DisableAllPreviews() {
        for (int i = 0; i < possibleSandBlobs.Count; i++)
            possibleSandBlobs[i].previewObject.SetActive(false);
    }

    private void ChangeBlobType(SandBlobType sandBlobType) {
        currentSandBlobType = sandBlobType;
        currentSandBlob = possibleSandBlobs.Find(p => p.data.type == sandBlobType);
        DisableAllPreviews();
        Debug.Log("Changed to " + currentSandBlob.data.type);
    }

    private void AwaitInput() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeBlobType(SandBlobType.CUBE_1x1);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeBlobType(SandBlobType.SPHERE_1x1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeBlobType(SandBlobType.BLOB_1x1);
        } 
        else if (Input.GetKeyDown(KeyCode.Delete)) {
            ResetBuildZone();
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }

    private void AddSandBlob(GridIndex gridIndex, Vector3 spawnPosition) {

        //Creat clay object
        ClayObject clayObject = clayContainer.addClayObject();
        
        //Initalize blob with parameters
        InitializeSandBlob(clayObject, currentSandBlob, spawnPosition);
        clayObject.enabled = false;
        gridDrawer.AddObjectToGrid(gridIndex);
        clayContainer.forceUpdateAllSolids(); 
        clayContainer.enableAllClayObjects(true);
    }

    private void InitializeSandBlob(ClayObject clayObject, SandBlob sandBlobType, Vector3 position) {
        clayObject.transform.localScale = sandBlobType.data.size;
        clayObject.color = sandBlobType.data.color;
        clayObject.blend = sandBlobType.data.blend /100;
        clayObject.attrs = new Vector4(sandBlobType.data.round /100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(sandBlobType.data.primitiveShape);
        clayObject.transform.position = new Vector3(position.x, position.y+.4f, position.z); ;
    }
}
