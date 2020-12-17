using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandcastleUI : MonoBehaviour
{
    public GameObject uiPanel;
    public KeyCode resetBuildZone, toggleUI, toggleBigBlobs, undo, redo, toggleRemove;
    public KeyCode buildCube, buildSphere, buildBlob;

    public Action<SandBlobType> SandBlobChanged;
    public Action<OperationType> BuildModeChanged;
    public Action ResetBuildZone, MouseClicked, UndoLastAction, RedoAction;

    public bool bigBlobs = false;
    public bool removeModeActive = false;

    public void CheckInput(SandBlobType sandBlobType) {

        if (!removeModeActive) {

            //Change blob size
            if (Input.GetKeyDown(toggleBigBlobs)) {
                bigBlobs = !bigBlobs;
                switch (sandBlobType) {
                    case SandBlobType.CUBE_1x1:
                        SandBlobChanged?.Invoke(SandBlobType.CUBE_3x1);
                        break;
                    case SandBlobType.SPHERE_1x1:
                        SandBlobChanged?.Invoke(SandBlobType.CYLINDER_3x1);
                        break;
                    case SandBlobType.BLOB_1x1:
                        SandBlobChanged?.Invoke(SandBlobType.BLOB_3x1);
                        break;
                    case SandBlobType.BLOB_3x1:
                        SandBlobChanged?.Invoke(SandBlobType.BLOB_1x1);
                        break;
                    case SandBlobType.CUBE_3x1:
                        SandBlobChanged?.Invoke(SandBlobType.CUBE_1x1);
                        break;
                    case SandBlobType.CYLINDER_3x1:
                        SandBlobChanged?.Invoke(SandBlobType.SPHERE_1x1);
                        break;
                    default:
                        break;
                }
            }

            //Change blob type
            if (bigBlobs) {
                if (Input.GetKeyDown(buildCube)) {
                    SandBlobChanged?.Invoke(SandBlobType.CUBE_3x1);
                } else if (Input.GetKeyDown(buildSphere)) {
                    SandBlobChanged?.Invoke(SandBlobType.CYLINDER_3x1);
                } else if (Input.GetKeyDown(buildBlob)) {
                    SandBlobChanged?.Invoke(SandBlobType.BLOB_3x1);
                }
            } else {
                if (Input.GetKeyDown(buildCube)) {
                    SandBlobChanged?.Invoke(SandBlobType.CUBE_1x1);
                } else if (Input.GetKeyDown(buildSphere)) {
                    SandBlobChanged?.Invoke(SandBlobType.SPHERE_1x1);
                } else if (Input.GetKeyDown(buildBlob)) {
                    SandBlobChanged?.Invoke(SandBlobType.BLOB_1x1);
                }
            }
        }

        //Change operation type
        if (Input.GetKeyDown(toggleRemove)) {
            removeModeActive = !removeModeActive;
            if (removeModeActive) {
                SandBlobChanged?.Invoke(SandBlobType.CUBE_1x1);
                BuildModeChanged?.Invoke(OperationType.SUBTRACT);
            } else {
                BuildModeChanged?.Invoke(OperationType.ADD);
            }
        }

        //Check other key inputs
        if (Input.GetKeyDown(resetBuildZone)) {
            ResetBuildZone?.Invoke();
        } else if (Input.GetKeyDown(toggleUI)) {
            ToggleUIPanel(!uiPanel.activeSelf);
        } else if (Input.GetKeyDown(undo)) {
            UndoLastAction?.Invoke();
        } else if (Input.GetKeyDown(redo)) {
            RedoAction?.Invoke();
        }

        //Detect mouse click
        if (Input.GetMouseButtonDown(0))
            MouseClicked?.Invoke();
    }

    public void Undo() {
        UndoLastAction?.Invoke();
    }

    public void Redo() {
        RedoAction?.Invoke();
    }

    public void DeleteAll() {
        ResetBuildZone?.Invoke();
    }

    private void ToggleUIPanel(bool on) {
        uiPanel.SetActive(on);
    }
}
