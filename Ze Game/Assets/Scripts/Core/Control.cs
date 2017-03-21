using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Control : MonoBehaviour {

	public static Control script;
	private int chosenDifficulty;
	private int attempt;
	public bool isNewGame = true;

	private SaveData loadedData;


	void Awake() {
		if (script == null) {
			script = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (script != this) {
			Destroy(gameObject);
		}

		if (!Directory.Exists(Application.dataPath + "/Saves")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D0")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D0");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D1")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D1");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D2")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D2");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D3")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D3");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D4")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D4");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D0/Resources")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D0/Resources");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D1/Resources")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D1/Resources");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D2/Resources")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D2/Resources");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D3/Resources")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D3/Resources");
		}
		if (!Directory.Exists(Application.dataPath + "/Saves/D4/Resources")) {
			Directory.CreateDirectory(Application.dataPath + "/Saves/D4/Resources");
		}
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}



	public void Save(bool newsaveFile) {
		print("Saving");

		if (newsaveFile) {
			attempt++;
			PlayerPrefs.SetInt("A", attempt);
		}

		chosenDifficulty = PlayerPrefs.GetInt("difficulty");
		attempt = PlayerPrefs.GetInt("A");

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.dataPath + "/Saves/D" + chosenDifficulty + "/Save-D" + chosenDifficulty + "_" + attempt.ToString("000") + ".Kappa");
		SaveData data = new SaveData();
		Statics.gameProgression.GetValues();

		data.coinsCollected = Coins.coinsCollected;
		data.spikesCollected = Spike.spikesCollected;
		data.bombs = PlayerAttack.bombs;
		data.bullets = PlayerAttack.bullets;
		data.playerPositionX = Statics.gameProgression.currentPositionPlayerX;
		data.playerPositionY = Statics.gameProgression.currentPositionPlayerY;
		data.playerPositionZ = Statics.gameProgression.currentPositionPlayerZ;
		data.blockPosX = Statics.gameProgression.currentPositionBoxX;
		data.blockPosY = Statics.gameProgression.currentPositionBoxY;
		data.blockPosZ = Statics.gameProgression.currentPositionBoxZ;
		data.blockZRotation = Statics.gameProgression.ZRotationBlock;
		data.difficulty = chosenDifficulty;
		data.currentBGName = M_Player.currentBG_name;
		data.currentlyDisplayedSideInfo = Statics.canvasRenderer.info_S.text;
		data.time = Mathf.Round(timer.time * 1000) / 1000;
		data.shownAttempt = Statics.mPlayer.newGame;
		data.shownShotInfo = Statics.playerAttack.displayShootingInfo;
		data.shownAvoidanceInfo = Statics.avoidance.displayAvoidInfo;
		data.doneAvoidance = Statics.avoidance.preformed;
		data.shownBlockInfo = Statics.blockScript.showInfo;
		data.blockPushAttempt = Statics.pressurePlate.attempts;
		data.camSize = Camera.main.orthographicSize;
		data.canZoom = Statics.zoom.canZoom;
		data.bossSpawned = Statics.cameraMovement.inBossRoom;

		formatter.Serialize(file, data);
		file.Close();
		StartCoroutine(ScreenShot(attempt));


		

	}


	private IEnumerator ScreenShot(int currAttempt) {
		GameObject saveButton = GameObject.Find("saveGame");
		if (saveButton != null) {
			yield return new WaitUntil(() => !saveButton.activeInHierarchy);
		}
		print("Captured");
		Application.CaptureScreenshot(Application.dataPath + "/Saves/D" + chosenDifficulty + "/Resources/Save-D" + chosenDifficulty + "_" + currAttempt.ToString("000") + ".png");
	}

	public void Load(string fileToLoad) {

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(fileToLoad, FileMode.Open);


		SaveData data = (SaveData)bf.Deserialize(file);
		file.Close();

		loadedData = data;
		StartCoroutine(FilesLoaded());
	}

	private IEnumerator FilesLoaded() {
		Statics.camFade.PlayTransition("Trans");
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(1);
		isNewGame = false;
	}


	private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.buildIndex == 1 && !isNewGame) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			GameObject block = GameObject.Find("Block");


			Vector3 playerPos = new Vector3(loadedData.playerPositionX, loadedData.playerPositionY, loadedData.playerPositionZ);
			player.transform.position = playerPos;

			Coins.coinsCollected = loadedData.coinsCollected;
			Spike.spikesCollected = loadedData.spikesCollected;
			Statics.canvasRenderer.Counters("Update");

			PlayerAttack.bullets = loadedData.bullets;
			PlayerAttack.bombs = loadedData.bombs;
			Statics.playerAttack.displayShootingInfo = loadedData.shownShotInfo;

			Statics.blockScript.showInfo = loadedData.shownBlockInfo;
			block.transform.position = new Vector3(loadedData.blockPosX, loadedData.blockPosY, loadedData.blockPosZ);
			block.transform.rotation = Quaternion.AngleAxis(loadedData.blockZRotation, Vector3.back);
			if (loadedData.blockPushAttempt == 3) {
				Statics.pressurePlate.CreateBarrier();
			}
			else {
				Statics.pressurePlate.attempts = loadedData.blockPushAttempt;
			}

			if (!loadedData.shownShotInfo) {
				Statics.playerAttack.UpdateStats();
			}
			if (loadedData.coinsCollected == 5) {
				Statics.coins.ChatchUpToAttempt(loadedData.coinsCollected - 2);
				GameObject.Find("Coin").SetActive(false);
				Statics.coins.CoinBehavior();
			}
			else if (loadedData.coinsCollected <= 4) {
				Statics.coins.ChatchUpToAttempt(loadedData.coinsCollected - 2);
				Statics.coins.CoinBehavior();
				Statics.guide.Recalculate(GameObject.Find("Coin"), true);
			}


			PlayerPrefs.SetInt("difficulty", loadedData.difficulty);
			M_Player.gameProgression = loadedData.spikesCollected;
			Statics.gameProgression.Progress();
			Statics.mPlayer.newGame = false;

			Statics.avoidance.displayAvoidInfo = loadedData.shownAvoidanceInfo;
			Statics.avoidance.preformed = loadedData.doneAvoidance;


			timer.time = loadedData.time;
			timer.run = true;

			Camera.main.orthographicSize = loadedData.camSize;
			Statics.zoom.canZoom = loadedData.canZoom;

			Statics.cameraMovement.inBossRoom = loadedData.bossSpawned;
			if (loadedData.bossSpawned) {
				Statics.bossEntrance.SpawnBossOnLoad();

			}

			Statics.canvasRenderer.infoRenderer(null, loadedData.currentlyDisplayedSideInfo);

			switch (loadedData.currentBGName) {
				case "Background_Start": {
					Statics.music.PlayMusic(Statics.music.room1);
					break;
				}
				case "Background_room_2b": {
					Statics.music.PlayMusic(Statics.music.room1);
					break;
				}
				case "Background_room_Boss_1": {
					Statics.music.PlayMusic(Statics.music.boss);
					break;
				}
				case "MazeBG": {
					Statics.music.PlayMusic(Statics.music.maze);
					break;
				}
				default: {
					break;
				}
			}
		}
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}
}

//Data to be saved
[Serializable]
public class SaveData {
	public int coinsCollected;
	public int spikesCollected;
	public int bullets;
	public int bombs;

	public float playerPositionX, playerPositionY, playerPositionZ;
	public float blockPosX, blockPosY, blockPosZ;
	public float blockZRotation;

	public int difficulty;
	public float time;
	public bool canZoom;

	public string currentBGName;
	public string currentlyDisplayedSideInfo;

	public bool shownShotInfo;
	public bool shownAttempt;
	public bool shownBlockInfo;
	public int blockPushAttempt;

	public float camSize;



	public bool shownAvoidanceInfo;
	public bool doneAvoidance;
	public bool bossSpawned;

}
//Static reference to other classes
public class Statics : MonoBehaviour {

	public static BossBehaviour bossBehaviour;
	public static BossEntrance bossEntrance;
	public static BossHealth bossHealth;


	public static SwitchScene switchScene;


	public static Coins coins;
	public static Guide guide;
	public static Spike spike;


	public static CameraMovement cameraMovement;
	public static Canvas_Renderer canvasRenderer;
	public static GameProgression gameProgression;
	public static PauseUnpause pauseUnpause;
	public static SaveGame saveGame;
	public static timer timerScript;
	public static Wrapper wrapper;
	public static Zoom zoom;


	public static EnemySpawner enemySpawner;
	public static Projectile projectile;
	public static TurretAttack turretAttack;


	public static Maze mazeScript;
	public static MazeEntrance mazeEntrance;
	public static MazeEscape mazeEscape;


	public static M_Player mPlayer;
	public static PlayerAttack playerAttack;
	public static SpikeBullet spikeBullet;


	public static Avoidance avoidance;
	public static BlockScript blockScript;
	public static PressurePlate pressurePlate;


	public static CamFadeOut camFade;
	public static MusicHandler music;
	public static SoundFXHandler sound;

	public static DisplaySaveFiles displaySaves;

}
