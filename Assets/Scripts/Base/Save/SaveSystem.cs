using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
	private static readonly SaveSystem instance = new SaveSystem();
    public static SaveSystem Instance => instance;

    private SaveData saveData = new SaveData();
    public SaveData SaveData { get => saveData; }

    public void SavePlayerPosition(Vector3 newPosition) {
		if (saveData == null)
			saveData = new SaveData();
		saveData.playerPosition = newPosition;
	}

	public void ClearSave() {
		saveData = new SaveData();
		SaveGame();
	}

	public void SaveGame() {
		
		if (saveData == null)
			saveData = new SaveData();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, saveData);
		file.Close();

		Debug.Log("Game Saved");
	}

	public void LoadGame() {
		
		if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
			
			//Load file
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			saveData = (SaveData)bf.Deserialize(file);
			file.Close();

			//Set data
			GameUtil.Player.transform.position = saveData.playerPosition;
			
			Debug.Log("Game Loaded");
		}
		else
		{
			Debug.Log("No game saved!");
		}
	}
}
