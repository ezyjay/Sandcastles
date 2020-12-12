using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandCastleUI : MonoBehaviour
{
    public GameObject uiPanel;
    public SandcastleManager sandcastleManager;

    public bool bigBlobs = false;

    private void AwaitInput() {

        if (Input.GetKeyDown(KeyCode.T)) {
            bigBlobs = !bigBlobs;
            switch (sandcastleManager.currentSandBlobType) {
                case SandBlobType.CUBE_1x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.CUBE_3x1);
                    break;
                case SandBlobType.SPHERE_1x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.CYLINDER_3x1);
                    break;
                case SandBlobType.BLOB_1x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.BLOB_3x1);
                    break;
                case SandBlobType.BLOB_3x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.BLOB_1x1);
                    break;
                case SandBlobType.CUBE_3x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.CUBE_1x1);
                    break;
                case SandBlobType.CYLINDER_3x1:
                    sandcastleManager.ChangeBlobType(SandBlobType.SPHERE_1x1);
                    break;
                default:
                    break;
            }
        }

        if (bigBlobs) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                sandcastleManager.ChangeBlobType(SandBlobType.CUBE_3x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                sandcastleManager.ChangeBlobType(SandBlobType.CYLINDER_3x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                sandcastleManager.ChangeBlobType(SandBlobType.BLOB_3x1);
            } 
        }
        else {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                sandcastleManager.ChangeBlobType(SandBlobType.CUBE_1x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                sandcastleManager.ChangeBlobType(SandBlobType.SPHERE_1x1);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                sandcastleManager.ChangeBlobType(SandBlobType.BLOB_1x1);
            }
        }


        if (Input.GetKeyDown(KeyCode.Delete)) {
            sandcastleManager.ResetBuildZone();
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }

    private void Update() {
        AwaitInput();
    }
}
