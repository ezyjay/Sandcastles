using UnityEngine;
using Clayxels;
using static GridDrawer;

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public ClayContainer clayContainer;
    public SandBlobParams sandBlobParameters;
    public GridDrawer gridDrawer;
    public ClayObject sandBlobPreview;
    
    private RaycastHit hit;
    private Ray ray;

    [EditorButton]
    public void ResetBuildZone() {
        int i = 0;
        foreach (Transform clayObject in clayContainer.transform) {
            //Don't destroy 2 first children because first one is base and second is preview
            if (i > 1) {
                Destroy(clayObject.gameObject);
            }
            i++;
        }
        gridDrawer.ClearGrid();
    }

    [System.Serializable]
    public struct SandBlobParams {
        public Vector3 size;
        public Color color;
        public float blend;
        public float round;
        public int primitiveType;
    }

    private void OnEnable() {
        clayContainer.enableAllClayObjects(true);
        //Disable base clay object so it doesn't update every frame
        clayContainer.GetComponentInChildren<ClayObject>().enabled = false;
        sandBlobPreview.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isInBuildMode) {

            //Ray cast down and if it's a buildable area, spawn sand blob on mouse click
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {

                gridDrawer.MoveObjectOnGrid(sandBlobPreview.transform, hit.point);
                if (gridDrawer.IsPositionInGrid(sandBlobPreview.transform.position)) {

                    //Change position of preview object
                    GridIndex gridIndex = gridDrawer.GetTileIndexFromPosition(sandBlobPreview.transform.position);
                    Vector3 spawnPosition = new Vector3(sandBlobPreview.transform.position.x, transform.position.y, sandBlobPreview.transform.position.z);

                    if (gridDrawer.HasObjectInTile(gridIndex)) {
                        float newYPosition = spawnPosition.y + gridDrawer.GetNumberOfObjectsInTile(gridIndex) * gridDrawer.tilesize;
                        spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                        sandBlobPreview.transform.position = new Vector3(sandBlobPreview.transform.position.x, spawnPosition.y - .6f * gridDrawer.tilesize, sandBlobPreview.transform.position.z);
                    }
                    else {
                        sandBlobPreview.transform.position = new Vector3(sandBlobPreview.transform.position.x, transform.position.y, sandBlobPreview.transform.position.z);
                    }

                    sandBlobPreview.gameObject.SetActive(true);

                    //Add sand blobl on click
                    if (Input.GetMouseButtonDown(0)) 
                        AddSandBlob(gridIndex, spawnPosition);
                }
                else {
                    clayContainer.enableAllClayObjects(true);
                    sandBlobPreview.gameObject.SetActive(false);
                }
            }
            else {
                clayContainer.enableAllClayObjects(true);
                sandBlobPreview.gameObject.SetActive(false);
            }

        }
    }

    private void AddSandBlob(GridIndex gridIndex, Vector3 spawnPosition) {

        //Creat clay object
        ClayObject clayObject = clayContainer.addClayObject();
        
        //Initalize blob with parameters
        InitializeSandBlob(clayObject, sandBlobParameters, spawnPosition);
        clayObject.enabled = false;
        gridDrawer.AddObjectToGrid(gridIndex);
        sandBlobPreview.transform.SetAsLastSibling();
        clayContainer.forceUpdateAllSolids(); 
        clayContainer.enableAllClayObjects(true);
    }

    private void InitializeSandBlob(ClayObject clayObject, SandBlobParams parameters, Vector3 position) {
        clayObject.transform.localScale = parameters.size;
        clayObject.color = parameters.color;
        clayObject.blend = parameters.blend/100;
        clayObject.attrs = new Vector4(parameters.round/100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(parameters.primitiveType);
        clayObject.transform.position = position;
    }
}
