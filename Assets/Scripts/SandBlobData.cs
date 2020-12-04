using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CustomData/SandBlobData", order = 1)]
public class SandBlobData : ScriptableObject
{
    public SandBlobType type = SandBlobType.CUBE_1x1;
    public Vector3 size = Vector3.one;
    public Color color = new Color(214, 171, 64, 255);
    public float blend = 100f;
    public int primitiveShape = 0; //0=cube, 1=sphere
    [ShowIf("primitiveShape", 0)]
    public float round = 0;
    //[ShowIf("primitiveShape", 1)]
    //public float sliceY = 0;

    private void OnValidate() {
        
        if ((int)type >= 0 && (int)type < 100) {
            size = Vector3.one;
        }
        else if ((int)type >= 100) {
            size = new Vector3(3, 1, 3);
        }
    }
}

[System.Serializable]
public enum SandBlobType
{
    // 0-100: 1x1 shapes
    CUBE_1x1 = 0,
    SPHERE_1x1 = 1,
    BLOB_1x1 = 2,

    // 100-200: 3x1 shapes
    BLOB_3x1 = 100,
    CUBE_3x1 = 101,
    CYLINDER_3x1 = 102,
}