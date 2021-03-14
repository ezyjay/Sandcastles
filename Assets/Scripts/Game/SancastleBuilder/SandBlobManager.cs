using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SandBlob
{
    public SandBlobData data;
    public GameObject previewObject;
}

public class SandBlobManager : MonoBehaviour
{
    public Material goodPositionMat, badPositionMat;
    public List<SandBlob> possibleSandBlobs = new List<SandBlob>();

    public SandBlob CurrentSandBlob { get => currentSandBlob; set => currentSandBlob = value; }
    public GameObject CurrentSandBlobPreview { get => currentSandBlob.previewObject; }
    public GameObject CurrentTempRemoveObject { get => currentTempRemoveObject; set => currentTempRemoveObject = value; }

    private SandBlob currentSandBlob;
    private MeshRenderer currentBlobMeshRenderer;
    private GameObject currentTempRemoveObject;

    public void Initialize(SandBlobType sandBlobType) {
        DisableAllPreviews();
        ChangeBlobType(sandBlobType);
        BlobAtCorrectPosition(true);
    }

    public SandBlobType ChangeBlobType(SandBlobType sandBlobType) {
        currentSandBlob = possibleSandBlobs.Find(p => p.data.type == sandBlobType);
        currentBlobMeshRenderer = CurrentSandBlobPreview.GetComponentInChildren<MeshRenderer>();
        DisableAllPreviews();
        //Debug.Log("Changed to " + currentSandBlob.data.type);
        return sandBlobType;
    }

    public void DisableAllPreviews() {
        for (int i = 0; i < possibleSandBlobs.Count; i++)
            possibleSandBlobs[i].previewObject.SetActive(false);
    }

    public void BlobAtCorrectPosition(bool isCorrectPosition) {
        if (isCorrectPosition)
            currentBlobMeshRenderer.material = goodPositionMat;
        else
            currentBlobMeshRenderer.material = badPositionMat;
    }

    public void ToggleBlobPreview(bool on) {
        CurrentSandBlobPreview.SetActive(on);
    }

    public void EnableSandBlobCollider(Transform parent) {
        GameObject templateColliderObject = CurrentSandBlobPreview.GetComponentInChildren<Rigidbody>(true).gameObject;
        GameObject instanciatedColliderObject = Instantiate(templateColliderObject, parent);

        instanciatedColliderObject.SetActive(true);
    }

    public void SetBlobPreviewYPosition(float position) {
        CurrentSandBlobPreview.transform.position = new Vector3(CurrentSandBlobPreview.transform.position.x, position, CurrentSandBlobPreview.transform.position.z);
    }

    public Vector3 GetSpawnPosition() {
        return new Vector3(CurrentSandBlobPreview.transform.position.x, transform.position.y, CurrentSandBlobPreview.transform.position.z);
    }
}
