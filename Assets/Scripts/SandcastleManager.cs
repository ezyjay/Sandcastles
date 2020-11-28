using UnityEngine;
using Clayxels;

public class SandcastleManager : MonoBehaviour
{
    public bool isInBuildMode = false;
    public ClayContainer clayContainer;
    public SandBlobParams sandBlobParameters;
    public GridDrawer gridDrawer;
    
    private RaycastHit hit;
    private Ray ray;


[System.Serializable]
    public struct SandBlobParams {
        public Vector3 size;
        public Color color;
        public float blend;
        public float round;
        public int primitiveType;
    }
    
    private void Update()
    {
        if (isInBuildMode) {

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                gridDrawer.previewSprite.enabled = true;
                gridDrawer.MovePreviewSprite(hit.point);
            }
            else {
                gridDrawer.previewSprite.enabled = false;
            }

            if (Input.GetMouseButtonDown(0)) {

                
                ClayObject clayObject = clayContainer.addClayObject();
                InitializeBlob(clayObject, sandBlobParameters, gridDrawer.previewSprite.transform.position);
                clayContainer.enableAllClayObjects(true);
            }
        }
    }

    private void InitializeBlob(ClayObject clayObject, SandBlobParams parameters, Vector3 position) {
        clayObject.transform.localScale = parameters.size;
        clayObject.color = parameters.color;
        clayObject.blend = parameters.blend/100;
        clayObject.attrs = new Vector4(parameters.round/100, clayObject.attrs.y, clayObject.attrs.z, clayObject.attrs.w);
        clayObject.setPrimitiveType(parameters.primitiveType);
        clayObject.transform.position = position;
    }
}
