using Clayxels;
using System.Collections.Generic;
using UnityEngine;
using static GridDrawer;

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public ClayContainer clayContainer;
    public GridDrawer gridDrawer;
    public Material goodPositionMat, badPositionMat;
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
        ChangePreviewBlobMaterial(goodPositionMat);
    }

    private void Update()
    {
        ChangePreviewBlobMaterial(goodPositionMat);
        if (isInBuildMode) {

            //Ray cast down and if it's a buildable area, spawn sand blob on mouse click
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            if (Physics.Raycast(ray, out hit)) {

                gridDrawer.MoveObjectOnGrid(currentSandBlob.previewObject.transform, hit.point);
                if (gridDrawer.IsInsideGridBounds(currentSandBlob.previewObject.transform.localPosition)) {

                    //Change position of preview object
                    Vector2Int gridIndex = gridDrawer.GetTileIndexFromPosition(currentSandBlob.previewObject.transform.position);
                    Vector3 spawnPosition = new Vector3(currentSandBlob.previewObject.transform.position.x, transform.position.y, currentSandBlob.previewObject.transform.position.z);

                    List<Vector2Int> indexes = GetShapeGridIndexes(gridIndex);

                    if (gridDrawer.IndexesInsideGridBounds(indexes) && gridDrawer.IndexesHaveSameValue(indexes)) {

                        if (gridDrawer.HasObjectInTile(gridIndex)) {
                            float newYPosition = spawnPosition.y + gridDrawer.GetNumberOfObjectsInTile(gridIndex) * gridDrawer.tilesize;
                            spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                            currentSandBlob.previewObject.transform.position = new Vector3(currentSandBlob.previewObject.transform.position.x, spawnPosition.y - .2f * gridDrawer.tilesize, currentSandBlob.previewObject.transform.position.z);
                        } else {
                            currentSandBlob.previewObject.transform.position = new Vector3(currentSandBlob.previewObject.transform.position.x, transform.position.y, currentSandBlob.previewObject.transform.position.z);
                        }

                        currentSandBlob.previewObject.gameObject.SetActive(true);

                        //Add sand blob on click
                        if (Input.GetMouseButtonDown(0))
                            AddSandBlob(gridIndex, spawnPosition);
                    }
                    else {
                        clayContainer.enableAllClayObjects(true);
                        ChangePreviewBlobMaterial(badPositionMat);
                    }
                }
                else {
                    clayContainer.enableAllClayObjects(true);
                    currentSandBlob.previewObject.SetActive(false);
                }
            }
            else {
                clayContainer.enableAllClayObjects(true);
                currentSandBlob.previewObject.SetActive(false);
            }

        }
    }

    private void ChangePreviewBlobMaterial(Material material) {
        currentSandBlob.previewObject.GetComponentInChildren<MeshRenderer>().material = material;
    }

    private void DisableAllPreviews() {
        for (int i = 0; i < possibleSandBlobs.Count; i++)
            possibleSandBlobs[i].previewObject.SetActive(false);
    }

    public void ChangeBlobType(SandBlobType sandBlobType) {
        currentSandBlobType = sandBlobType;
        currentSandBlob = possibleSandBlobs.Find(p => p.data.type == sandBlobType);
        DisableAllPreviews();
        Debug.Log("Changed to " + currentSandBlob.data.type);
    }

    private void AddSandBlob(Vector2Int gridIndex, Vector3 spawnPosition) {

        //Creat clay object
        ClayObject clayObject = clayContainer.addClayObject();
        
        //Initalize blob with parameters
        InitializeSandBlob(clayObject, currentSandBlob, spawnPosition);

        //Spawn collider to be able to detect mouse over the blob
        GameObject colliderObject = currentSandBlob.previewObject.GetComponentInChildren<Rigidbody>(true).gameObject;
        GameObject instanciatedColliderObject = Instantiate(colliderObject, clayObject.transform);
        instanciatedColliderObject.SetActive(true);

        clayObject.enabled = false;

        //Updade grid and clay objects
        UpdateGridIndex(gridIndex);
        clayContainer.forceUpdateAllSolids(); 
        clayContainer.enableAllClayObjects(true);
    }

    private void UpdateGridIndex(Vector2Int middleGridIndex) {

        List<Vector2Int> indexes = GetShapeGridIndexes(middleGridIndex);

        //Fill grid with found indexes
        foreach (Vector2Int index in indexes)
            gridDrawer.AddObjectToGrid(index);
    }

    private List<Vector2Int> GetShapeGridIndexes(Vector2Int middleGridIndex) {

        List<Vector2Int> indexes = new List<Vector2Int>();

        //Just update current grid index if the shape is of size 1
        if (currentSandBlob.data.size == Vector3.one) {
            indexes.Add(middleGridIndex);
        } else {

            //If cylinder shaper, we only want to fill in a cross shape
            bool fillDiagonalNeighbours = true;
            if (currentSandBlob.data.type == SandBlobType.CYLINDER_3x1)
                fillDiagonalNeighbours = false;

            Vector2Int amountToAdd = new Vector2Int((int)currentSandBlob.data.size.x / 2, (int)currentSandBlob.data.size.z / 2);

            for (int i = middleGridIndex.x - amountToAdd.x; i <= middleGridIndex.x + amountToAdd.x; i++) {
                for (int j = middleGridIndex.y - amountToAdd.y; j <= middleGridIndex.y + amountToAdd.y; j++) {

                    Vector2Int currentIndex = new Vector2Int(i, j);
                    //If we don't want to fill diagonal neighbours check that the distance is the same as the amount to add
                    if (fillDiagonalNeighbours || Vector2Int.Distance(currentIndex, middleGridIndex) == amountToAdd.x || currentIndex == middleGridIndex) {
                        //int xPosition = Mathf.Clamp(i, 0, gridDrawer.maxGridSize - 1);
                        //int yPosition = Mathf.Clamp(j, 0, gridDrawer.maxGridSize - 1);
                        //if (gridDrawer.IsInsideGridBounds(currentIndex))
                            indexes.Add(currentIndex);// new Vector2Int(xPosition, yPosition));
                    }
                }
            }
        }

        return indexes;
    }

    private void InitializeSandBlob(ClayObject clayObject, SandBlob sandBlob, Vector3 position) {
        clayObject.transform.localScale = sandBlob.data.size;
        clayObject.color = sandBlob.data.color;
        clayObject.blend = sandBlob.data.blend /100;
        clayObject.attrs = new Vector4(sandBlob.data.round /100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(sandBlob.data.primitiveShape);
        clayObject.transform.position = new Vector3(position.x, position.y+.4f, position.z); ;
    }
}
