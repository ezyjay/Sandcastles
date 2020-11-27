using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject uiPanel;
	public Vector3 playerStartPosition;

	public bool loadSave = true;

	[EditorButton]
	public void ResetSaveData() {
		SaveSystem.Instance.SavePlayerPosition(playerStartPosition);
		SaveSystem.Instance.SaveGame();
	}

	private void Awake()
	{
		if (loadSave)
			SaveSystem.Instance.LoadGame();
			
		GameUtil.GameOver += OnGameOver;
		RenderSettings.fog = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (!uiPanel.activeSelf) {
				uiPanel.SetActive(true);
				Time.timeScale = 0;
			}
			else {
				uiPanel.SetActive(false);
				Time.timeScale = 1;
			}
		}
	}

	public void ExitGame() {
		Application.Quit();
	}

	private void OnDestroy() {
		GameUtil.GameOver -= OnGameOver;
	}

	public void LoadLevel(int level) {
		uiPanel.SetActive(false);
		Time.timeScale = 1;
       SceneManager.LoadScene(level);
	}

	public void OnGameOver() {
		uiPanel.SetActive(true);
	}
}
