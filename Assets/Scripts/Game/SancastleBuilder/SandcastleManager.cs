using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SandBlob
{
    public SandBlobData data;
    public GameObject previewObject;
}

[System.Serializable]
public enum OperationType
{
    ADD = 0,
    REMOVE = 1,
}

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public GridDrawer gridDrawer;
    public SandBlobManager sandBlobManager;
    public SandClayObjects sandClayObjects;
    public SandcastleUI sandcastleUI;
    //public OperationType currentOperationType = OperationType.ADD;
    public SandBlobType currentSandBlobType = SandBlobType.CUBE_1x1;

    private RaycastHit hit;
    private Ray ray;
    private bool canBuild = false;
    private Vector3 spawnPosition = Vector3.zero;
    private Vector2Int gridIndexSpawn = Vector2Int.zero;
    private List<Vector2Int> shapeIndexes = new List<Vector2Int>();

    [EditorButton]
    public void ResetBuildZone() {
        sandClayObjects.DestroyAllSolids();
        gridDrawer.ClearGrid();
    }

    private void Awake() {
        if (sandBlobManager == null)
            sandBlobManager = GetComponent<SandBlobManager>();

        if (sandcastleUI == null)
            sandcastleUI = GetComponentInChildren<SandcastleUI>();

        if (sandClayObjects == null)
            sandClayObjects = GetComponent<SandClayObjects>();
    }

    private void OnValidate() {
#if UNITY_EDITOR
        sandBlobManager.ChangeBlobType(currentSandBlobType);
#endif
    }

    private void OnEnable() {
        sandcastleUI.SandBlobChanged += OnCurrentSandBlobChanged;
        sandcastleUI.ResetBuildZone += ResetBuildZone;
        sandcastleUI.MouseClicked += OnMouseClicked;
        sandcastleUI.UndoLastAction += UndoLastAction;
        sandcastleUI.RedoAction += RedoAction;

        sandClayObjects.Initialize();
        sandBlobManager.Initialize(currentSandBlobType);
    }

    private void OnDisable() {
        sandcastleUI.SandBlobChanged -= OnCurrentSandBlobChanged;
        sandcastleUI.ResetBuildZone -= ResetBuildZone;
        sandcastleUI.MouseClicked -= OnMouseClicked;
        sandcastleUI.UndoLastAction -= UndoLastAction;
        sandcastleUI.RedoAction -= RedoAction;
    }

    private void Update()
    {
        if (isInBuildMode) {

            //UI
            sandcastleUI.CheckInput(currentSandBlobType);

            //Reset blob preview and force show solids
            canBuild = false;
            sandClayObjects.ToggleSolids(true);
            sandBlobManager.BlobAtCorrectPosition(false);
            sandBlobManager.ToggleBlobPreview(false);

            //Ray cast down and if it's a buildable area, spawn sand blob on mouse click
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            if (Physics.Raycast(ray, out hit)) {

                //Move sandBlobPreview on the grid
                gridDrawer.MoveObjectOnGrid(sandBlobManager.CurrentSandBlobPreview.transform, hit.point);
                
                //Check if it's inside grid bounds
                if (gridDrawer.IsInsideGridBounds(sandBlobManager.CurrentSandBlobPreview.transform.localPosition)) {

                    //Show blob preview
                    sandBlobManager.ToggleBlobPreview(true);

                    //Change position on grid on blob preview and determine spawn position and the surface it covers
                    gridIndexSpawn = gridDrawer.GetTileIndexFromPosition(sandBlobManager.CurrentSandBlobPreview.transform.position);                 
                    spawnPosition = sandBlobManager.GetSpawnPosition();
                    shapeIndexes = GetShapeGridIndexes(gridIndexSpawn);

                    //Check if current grid position already has something in it to set the Y value of blob preview
                    if (gridDrawer.HasObjectAtIndex(gridIndexSpawn)) {
                        float newYPosition = spawnPosition.y + gridDrawer.GetValueAtIndex(gridIndexSpawn) * gridDrawer.tilesize;
                        spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                        sandBlobManager.SetBlobPreviewYPosition(newYPosition - .2f * gridDrawer.tilesize);
                    } else {
                        sandBlobManager.SetBlobPreviewYPosition(transform.position.y);
                    }

                    //Check if shape of blob is inside the grid and on a flat surface
                    if (gridDrawer.AreIndexesInsideGridBounds(shapeIndexes) && gridDrawer.IndexesHaveSameOrBiggerValueThanIndex(gridIndexSpawn, shapeIndexes)) {

                        //Can build
                        sandBlobManager.BlobAtCorrectPosition(true);
                        canBuild = true;
                    }
                }
            }
        }
    }

    private void OnMouseClicked() {
        if (canBuild) {
            AddSandBlob(gridIndexSpawn, spawnPosition);
            canBuild = false;
        }
    }

    private void OnCurrentSandBlobChanged(SandBlobType sandBlobType) {
        currentSandBlobType = sandBlobManager.ChangeBlobType(sandBlobType);
    }

    private void RedoAction() {
        Vector3 addedObjectPosition = sandClayObjects.Redo();
        if (addedObjectPosition != Vector3.zero) {
            Vector2Int addedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(addedObjectPosition);
            List<Vector2Int> addedObjectShapeIndexes = GetShapeGridIndexes(addedObjectGridIndex);
            gridDrawer.IncrementTileValueIndexes(addedObjectShapeIndexes, gridDrawer.GetValueAtIndex(addedObjectGridIndex));
        }
    }

    private void UndoLastAction() {
        Vector3 removedObjectPosition = sandClayObjects.Undo();
        if (removedObjectPosition != Vector3.zero) {
            Vector2Int removedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(removedObjectPosition);
            List<Vector2Int> removedObjectShapeIndexes = GetShapeGridIndexes(removedObjectGridIndex);
            gridDrawer.DecrementTileValueIndexes(removedObjectShapeIndexes);
        }
    }

    private void AddSandBlob(Vector2Int gridIndex, Vector3 spawnPosition) {

        //Create clay object
        Transform clayObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition);
        
        //Spawn collider on clay object to be able to detect mouse over the blob
        sandBlobManager.EnableSandBlobCollider(clayObject);

        //Updade grid and other clay objects
        gridDrawer.IncrementTileValueIndexes(shapeIndexes, gridDrawer.GetValueAtIndex(gridIndexSpawn));
        sandClayObjects.UpdateSolids();
    }

    private List<Vector2Int> GetShapeGridIndexes(Vector2Int middleGridIndex) {

        Vector2Int amountToAdd = new Vector2Int((int)sandBlobManager.CurrentSandBlob.data.size.x / 2, (int)sandBlobManager.CurrentSandBlob.data.size.z / 2);
        
        bool fillDiagonalNeighbours = true;
        if (sandBlobManager.CurrentSandBlob.data.type == SandBlobType.CYLINDER_3x1)
            fillDiagonalNeighbours = false;
        
        return gridDrawer.GetNeighbourIndexes(middleGridIndex, amountToAdd, fillDiagonalNeighbours); ;
    }

    
}
