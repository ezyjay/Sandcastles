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
    SUBTRACT = 1,
}

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public GridDrawer gridDrawer;
    public SandBlobManager sandBlobManager;
    public SandClayObjects sandClayObjects;
    public SandcastleUI sandcastleUI;
    public OperationType currentOperationType = OperationType.ADD;
    public SandBlobType currentSandBlobType = SandBlobType.CUBE_1x1;
    public int maxHoleSize = 2;

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

    private void ShowDebugInfo(string header, Vector2Int index) 
    {
#if UNITY_EDITOR
        //DEBUG
        Debug.Log(header + ": " + index + " ==> Value = " + gridDrawer.GetHeightAtIndex(index) + " // Vertical objects list = ");
        for (int i = gridDrawer.GetHeightAtIndex(index) -1; i >= 0; i--) {
            Debug.Log(i + ": " + gridDrawer.GetVerticalValuesAtIndex(index)[i]);
        }
        Debug.Log("------------------------------------------------------------------------------------------------------------------------");
#endif
    }

    private void OnEnable() {
        sandcastleUI.SandBlobChanged += OnCurrentSandBlobChanged;
        sandcastleUI.ResetBuildZone += ResetBuildZone;
        sandcastleUI.MouseClicked += OnMouseClicked;
        sandcastleUI.UndoLastAction += UndoLastAction;
        sandcastleUI.RedoAction += RedoAction;
        sandcastleUI.BuildModeChanged += OnBuildModeChanged;

        gridDrawer.gameObject.SetActive(true);

        sandClayObjects.Initialize();
        sandBlobManager.Initialize(currentSandBlobType);
    }

    private void OnDisable() {
        sandcastleUI.SandBlobChanged -= OnCurrentSandBlobChanged;
        sandcastleUI.ResetBuildZone -= ResetBuildZone;
        sandcastleUI.MouseClicked -= OnMouseClicked;
        sandcastleUI.UndoLastAction -= UndoLastAction;
        sandcastleUI.RedoAction -= RedoAction;
        sandcastleUI.BuildModeChanged -= OnBuildModeChanged;
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
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.red);
            if (Physics.Raycast(ray, out hit)) {

                //ADD
                if (currentOperationType == OperationType.ADD) {

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
                            float newYPosition = spawnPosition.y + gridDrawer.GetHeightAtIndex(gridIndexSpawn) * gridDrawer.tileSize;
                            spawnPosition = new Vector3(spawnPosition.x, newYPosition, spawnPosition.z);
                            sandBlobManager.SetBlobPreviewYPosition(newYPosition - .2f * gridDrawer.tileSize);
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

                //SUBSTRACT
                else if (currentOperationType == OperationType.SUBTRACT) {

                    //Move sandBlobPreview on the grid
                    gridDrawer.MoveObjectOnGrid(sandBlobManager.CurrentSandBlobPreview.transform, hit.point, true);

                    gridIndexSpawn = gridDrawer.GetTileIndexFromPosition(sandBlobManager.CurrentSandBlobPreview.transform.position);

                    if (sandBlobManager.CurrentTempRemoveObject != null) 
                        sandBlobManager.CurrentTempRemoveObject.transform.position = new Vector3(sandBlobManager.CurrentSandBlobPreview.transform.position.x, sandBlobManager.CurrentSandBlobPreview.transform.position.y + .5f, sandBlobManager.CurrentSandBlobPreview.transform.position.z);

                    //Check if it's inside grid bounds and there is an object at index
                    if (gridDrawer.IsInsideGridBounds(sandBlobManager.CurrentSandBlobPreview.transform.localPosition) && gridDrawer.HasObjectAtIndex(gridIndexSpawn)) {

                        //Show blob preview
                        sandBlobManager.ToggleBlobPreview(true);

                        //Check not leaving too many holes
                        if (gridDrawer.CanRemoveObjectAtVerticalIndex(gridIndexSpawn, gridDrawer.tileSize * maxHoleSize, (int)sandBlobManager.CurrentTempRemoveObject.transform.position.y)) {

                            //Can build
                            sandBlobManager.BlobAtCorrectPosition(true);
                            canBuild = true;
                        }
                    }
                }
            }
        }
    }

    private void OnMouseClicked() {
        if (canBuild) {
            AddSandBlob();
            canBuild = false;
        }
    }

    private void OnCurrentSandBlobChanged(SandBlobType sandBlobType) {
        currentSandBlobType = sandBlobManager.ChangeBlobType(sandBlobType);
    }

    private void RedoAction() {
        Vector3 addedObjectPosition = sandClayObjects.Redo(out bool lastRemovedObjectWasSubstract);
        if (addedObjectPosition != Vector3.zero) {
            Vector2Int addedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(addedObjectPosition);
            List<Vector2Int> addedObjectShapeIndexes = GetShapeGridIndexes(addedObjectGridIndex);
            if (lastRemovedObjectWasSubstract)
                gridDrawer.RemoveValueAtVerticalIndexes(addedObjectShapeIndexes, (int)addedObjectPosition.y);
            else
                gridDrawer.AddValueAtVerticalIndexes(addedObjectShapeIndexes, (int)addedObjectPosition.y);

            //DEBUG
            ShowDebugInfo("REDO", addedObjectGridIndex);
        }
    }

    private void UndoLastAction() {
        Vector3 removedObjectPosition = sandClayObjects.Undo(out bool lastObjectWasSubstract);
        if (removedObjectPosition != Vector3.zero) {
            Vector2Int removedObjectGridIndex = gridDrawer.GetTileIndexFromPosition(removedObjectPosition);
            List<Vector2Int> removedObjectShapeIndexes = GetShapeGridIndexes(removedObjectGridIndex);
            if (lastObjectWasSubstract)
                gridDrawer.AddValueAtVerticalIndexes(removedObjectShapeIndexes, (int)removedObjectPosition.y);
            else
                gridDrawer.RemoveValueAtVerticalIndexes(removedObjectShapeIndexes, (int)removedObjectPosition.y);

            //DEBUG
            ShowDebugInfo("UNDO", removedObjectGridIndex);
        }
    }

    private void OnBuildModeChanged(OperationType newOperationType) {
        currentOperationType = newOperationType;
        if (currentOperationType == OperationType.SUBTRACT) {
            sandBlobManager.CurrentTempRemoveObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition, currentOperationType, true).gameObject;
        }
        else if (currentOperationType == OperationType.ADD) {
            if (sandBlobManager.CurrentTempRemoveObject != null)
                Destroy(sandBlobManager.CurrentTempRemoveObject);
        }
    }

    private void AddSandBlob() {

        //ADD
        if (currentOperationType == OperationType.ADD) {
            //Create clay object
            Transform clayObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition);

            //Spawn collider on clay object to be able to detect mouse over the blob
            sandBlobManager.EnableSandBlobCollider(clayObject);

            //Updade grid and other clay objects
            gridDrawer.IncrementTileValueIndexes(shapeIndexes, gridDrawer.GetHeightAtIndex(gridIndexSpawn));

            //DEBUG
            ShowDebugInfo("ADD OBJECT", gridIndexSpawn);
        }
        //SUBSTRACT
        else if (currentOperationType == OperationType.SUBTRACT) {

            if (sandBlobManager.CurrentTempRemoveObject != null) {
                sandClayObjects.CommitClayObject(sandBlobManager.CurrentTempRemoveObject);
                gridDrawer.RemoveValueAtVerticalIndex(gridIndexSpawn, (int)sandBlobManager.CurrentTempRemoveObject.transform.position.y);
            }

            //New remove preview
            sandBlobManager.CurrentTempRemoveObject = sandClayObjects.AddClayObject(sandBlobManager.CurrentSandBlob.data, spawnPosition, currentOperationType, true).gameObject;

            //DEBUG
            ShowDebugInfo("SUBSTRACT OBJECT", gridIndexSpawn);
        }

        //Update solids
        sandClayObjects.UpdateSolids();
    }

    private List<Vector2Int> GetShapeGridIndexes(Vector2Int middleGridIndex, int addToAmountToAdd = 0) {

        Vector2Int amountToAdd = new Vector2Int((int)sandBlobManager.CurrentSandBlob.data.size.x / 2, (int)sandBlobManager.CurrentSandBlob.data.size.z / 2);

        if (addToAmountToAdd != 0)
            amountToAdd += addToAmountToAdd * Vector2Int.one;

        bool fillDiagonalNeighbours = true;
        if (sandBlobManager.CurrentSandBlob.data.type == SandBlobType.CYLINDER_3x1)
            fillDiagonalNeighbours = false;
        
        return gridDrawer.GetNeighbourIndexes(middleGridIndex, amountToAdd, fillDiagonalNeighbours); ;
    }

    
}
