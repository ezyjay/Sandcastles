using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandcastleUI : MonoBehaviour
{
    public GameObject uiPanel;

    public Action<SandBlobType> SandBlobChanged;
    public Action ResetBuildZone, MouseClicked;

    public bool bigBlobs = false;

    public void CheckInput(SandBlobType sandBlobType) {

        //Change blob size
        if (Input.GetKeyDown(KeyCode.T)) {
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
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SandBlobChanged?.Invoke(SandBlobType.CUBE_3x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SandBlobChanged?.Invoke(SandBlobType.CYLINDER_3x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SandBlobChanged?.Invoke(SandBlobType.BLOB_3x1);
            } 
        }
        else {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SandBlobChanged?.Invoke(SandBlobType.CUBE_1x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SandBlobChanged?.Invoke(SandBlobType.SPHERE_1x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SandBlobChanged?.Invoke(SandBlobType.BLOB_1x1);
            }
        }

        //Check other key inputs
        if (Input.GetKeyDown(KeyCode.Delete)) {
            ResetBuildZone?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleUIPanel(!uiPanel.activeSelf);
        }

        //Detect mouse click
        if (Input.GetMouseButtonDown(0))
            MouseClicked?.Invoke();
    }

    private void ToggleUIPanel(bool on) {
        uiPanel.SetActive(on);
    }
}
