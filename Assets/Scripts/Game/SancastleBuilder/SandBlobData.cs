using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CustomData/SandBlobData", order = 1)]
public class SandBlobData : ScriptableObject
{
    public SandBlobType type = SandBlobType.CUBE_1x1;
    public Vector3 size = Vector3.one;
    public Quaternion rotation = Quaternion.identity;
    public Color color = new Color(214, 171, 64, 255);
    public float blend = 100f;
    public int primitiveShape = 0; //0=cube, 1=sphere, 2=cylinder, 3=torus
    [ShowIf("primitiveShape", 0, 2)]
    public float round = 0;
    [ShowIf("primitiveShape", 2)]
    public float cone = 0;
    [ShowIf("primitiveShape", 3)]
    public float fat = 0;
    

    private void OnValidate() {
        
        if ((int)type >= 0 && (int)type < 100) {
            size = Vector3.one;
        }
        else if ((int)type >= 100 && (int)type < 200) {
            size = new Vector3(3, 1, 3);
        } 
        else if ((int)type >= 300 && (int)type < 300) {
            size = new Vector3(3, 3, 3);
        }

        if (type == SandBlobType.CUBE_1x1 || type == SandBlobType.CUBE_3x1 || type == SandBlobType.BLOB_1x1 || type == SandBlobType.BLOB_3x1) {
            primitiveShape = 0;
        } 
        else if (type == SandBlobType.SPHERE_1x1 || type == SandBlobType.SPHERE_3x3) {
            primitiveShape = 1;
        } 
        else if (type == SandBlobType.CYLINDER_1x1 || type == SandBlobType.CYLINDER_3x1) {
            primitiveShape = 2;
        } 
        else if (type == SandBlobType.TORUS_1x1) {
            primitiveShape = 2;
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
    CYLINDER_1x1 = 3,
    TORUS_1x1 = 4,
    CONE_1x1 = 5,

    // 100-200: 3x1 shapes
    BLOB_3x1 = 100,
    CUBE_3x1 = 101,
    CYLINDER_3x1 = 102,

    // 200-300: 3x3 shapes
    SPHERE_3x3 = 200,
    PRISM_3x3 = 201,
}