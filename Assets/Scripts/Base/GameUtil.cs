using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerType {
    COLOR_OBJECT = 9,
    PLAYER = 10,
    LEVEL = 11,
	HIDE_OBJECT = 12,
	USE_COLOR_OBJECT = 13
}

public class GameUtil : MonoBehaviour
{
    private static Player player;
    public static Player Player { 
        get {			
			if(player == null)
				player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();		
			
			return player;
		}
    }

    public static LayerMask GetLayerMask(LayerType layerType) {
        return LayerMask.GetMask(LayerMask.LayerToName((int)layerType));
    }

	public static event Action GameOver;
	public static void ActivateGameOver() {
		GameOver.Invoke();
	}

	public static bool IsColorEqual(Color a, Color b, float tolerance) {
		Vector3 vectorA = new Vector3(a.r, a.g, a.b);
		Vector3 vectorB = new Vector3(b.r, b.g, b.b);

		if (Vector3.Distance(vectorA, vectorB) < tolerance)
			return true;
		
		return false;
	}
}

[System.Serializable]
public class SerializableVector3 {
	public float x, y, z;

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
	{
		return new Vector3(rValue.x, rValue.y, rValue.z);
	}
	
	public static implicit operator SerializableVector3(Vector3 rValue)
	{
		return new SerializableVector3(rValue.x, rValue.y, rValue.z);
	}
}
