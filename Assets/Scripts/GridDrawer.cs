using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int maxGridSize = 10;
    public int tilesize = 1;
    public SpriteRenderer tile, previewSprite;

  
    public void MovePreviewSprite(Vector3 position) {
        
        float newXPos = tilesize * Mathf.Round(position.x / tilesize);
        float newZPos = tilesize * Mathf.Round(position.z / tilesize);
        Vector3 newPos = new Vector3(newXPos, previewSprite.transform.position.y, newZPos) + new Vector3(1, 0, 1) * tilesize / 2;
        previewSprite.transform.position = newPos;
    }
}
