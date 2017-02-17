using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {


	RectTransform killerblockBG;
	RectTransform arrowtrapBG;
	RectTransform killerWallBG;
	Transform enemy;
	public GameObject foundation;
	public GameObject[] arrowtrap;
	public GameObject deathBlock;
	public M_Player player;
	public bool amIHere = false;
	public bool forTheFirstTime = true;
	public GameObject warningObj;
	public GameObject EPPooler;
	

	void Start() {
		killerblockBG = GameObject.Find("Background_Start").GetComponent<RectTransform>();
		arrowtrapBG = GameObject.Find("Background_room_2a").GetComponent<RectTransform>();
		killerWallBG = GameObject.Find("Background_room_1").GetComponent<RectTransform>();
		enemy = GameObject.Find("Enemies").transform;
	}

	public void spawnArrowTrap() {
		if (forTheFirstTime == false) {
		//	print(forTheFirstTime + " Normal");

			foreach (GameObject zone in CameraMovement.loadedZones) {

				if (zone.Equals(arrowtrapBG.gameObject) && amIHere == true) {
					break;
				}

				else if (zone.Equals(arrowtrapBG.gameObject) && amIHere == false) {

					Vector3 pos = new Vector3(arrowtrapBG.position.x, arrowtrapBG.position.y, 0);
					float bgx = arrowtrapBG.sizeDelta.x / 2;
					float bgy = arrowtrapBG.sizeDelta.y / 2;
					arrowtrap = new GameObject[4];

					arrowtrap[0] = (GameObject)Instantiate(foundation, pos + new Vector3(bgx / 2, bgy / 2, 0), Quaternion.identity);
					arrowtrap[1] = (GameObject)Instantiate(foundation, pos + new Vector3(-bgx / 2, bgy / 2, 0), Quaternion.identity);
					arrowtrap[2] = (GameObject)Instantiate(foundation, pos + new Vector3(bgx / 2, -bgy / 2, 0), Quaternion.identity);
					arrowtrap[3] = (GameObject)Instantiate(foundation, pos + new Vector3(-bgx / 2, -bgy / 2, 0), Quaternion.identity);
					amIHere = true;

					foreach (GameObject trap in arrowtrap) {
						trap.name = "arrowtrap";
						trap.transform.SetParent(enemy);
					}
					break;
				}
				else {
					foreach (GameObject deltrap in arrowtrap) {
						Destroy(deltrap.gameObject);
					}
					amIHere = false;
				}
			}
		}
	}

	public void spawnAvoidance() {
		print("Avoidance");
		Vector3 pos = new Vector3(arrowtrapBG.position.x, arrowtrapBG.position.y, 0);
		float bgx = arrowtrapBG.sizeDelta.x / 2;
		float bgy = arrowtrapBG.sizeDelta.y / 2;
		arrowtrap = new GameObject[4];

		arrowtrap[0] = (GameObject)Instantiate(foundation, pos + new Vector3(bgx / 2, bgy / 2, 0), Quaternion.identity);
		arrowtrap[1] = (GameObject)Instantiate(foundation, pos + new Vector3(-bgx / 2, bgy / 2, 0), Quaternion.identity);
		arrowtrap[2] = (GameObject)Instantiate(foundation, pos + new Vector3(bgx / 2, -bgy / 2, 0), Quaternion.identity);
		arrowtrap[3] = (GameObject)Instantiate(foundation, pos + new Vector3(-bgx / 2, -bgy / 2, 0), Quaternion.identity);

		foreach (GameObject trap in arrowtrap) {
			trap.name = "arrowtrap";
			trap.transform.SetParent(enemy);
		}
		StartCoroutine("hold");
	}

	private IEnumerator hold() {
		yield return new WaitForSeconds(30);
		forTheFirstTime = false;
		foreach (GameObject deltrap in arrowtrap) {
			Destroy(deltrap.gameObject);

		}
	}


	public List<GameObject> Blocks = new List<GameObject>();
	public List<GameObject> Warnings = new List<GameObject>();

	float scale;
	Vector2 killerblockpos;
	bool CRunning = false;

	public void spawnKillerBlock() {

		for (int count = 0; count < (int)(Spike.spikesCollected + 5 * difficultySlider.difficulty); count++) {

			scale = Random.Range(0.8f, 5);

			GameObject warn = Instantiate(warningObj);
			GameObject block = Instantiate(deathBlock);


			Vector3 pos = KBPositions();

			block.transform.position = pos;

			block.transform.localScale = new Vector3(scale, scale, 0);
			warn.transform.localScale = new Vector3(scale / 2, scale / 2, 0);

			block.name = "killerblock " + count;
			warn.name = "Warning " + count;

			block.transform.SetParent(enemy);
			warn.transform.SetParent(enemy);

			Blocks.Add(block);
			Warnings.Add(warn);

		}
		if (CRunning == false) {
			StartCoroutine("KBCycle");
		}

	}

	public IEnumerator KBCycle() {
		CRunning = true;

		while (true) {

			
			for (int i = 0; i < Blocks.Count; i++) {
				Warnings[i].SetActive(false);
				Blocks[i].SetActive(true);
			}
			yield return new WaitForSeconds(2);

			for (int i = 0; i < Blocks.Count; i++) {
				Blocks[i].GetComponent<Animator>().SetTrigger("Despawn");
			}

			yield return new WaitForSeconds(0.2f);
			//print(Blocks.Count + " " + Warnings.Count);

			for (int i = 0; i < Warnings.Count; i++) {
				Blocks[i].GetComponent<Animator>().SetTrigger("Reset");
				Blocks[i].SetActive(false);
				Vector3 pos = KBPositions();
				Blocks[i].transform.position = pos;
				Warnings[i].transform.position = pos;
				Warnings[i].SetActive(true);
				Warnings[i].GetComponent<Animator>().SetTrigger("DespawnWarn");


			}
			yield return new WaitForSeconds(0.5f);


			if (M_Player.currentBG_name != killerblockBG.name) {
				for (int i = 0; i < Blocks.Count; i++) {
					//Blocks[i].GetComponent<Animator>().SetTrigger("Reset");
					Blocks[i].SetActive(false);
					CRunning = false;
					StopCoroutine("KBCycle");
				}
			}
		}
	}


	public IEnumerator Warn(Vector3 pos, Vector3 scale) {
		GameObject warning = Instantiate(warningObj);
		warning.transform.position = pos;
		warning.transform.localScale = scale;
		warning.SetActive(true);
		yield return new WaitForSeconds(1);
		warning.SetActive(false);
		StopCoroutine("Warn");
	}




	List<GameObject> KWProjectiles = new List<GameObject>();

	public void spawnKillerWall() {

		if (PlayerPrefs.GetInt("difficulty") <= 1) {


			for (int i = 0; i < 1; i++) {
				GameObject wallShot = EPPooler.GetComponent<ObjectPooler>().GetPool();
				wallShot.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);
				wallShot.transform.position = KWProjectilePositions();
				wallShot.transform.SetParent(enemy);
				wallShot.SetActive(true);
				KWProjectiles.Add(wallShot);
			}
		}
		if (PlayerPrefs.GetInt("difficulty") == 2) {
			for (int i = 0; i < 2; i++) {
				GameObject wallShot = EPPooler.GetComponent<ObjectPooler>().GetPool();
				wallShot.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);
				wallShot.transform.position = KWProjectilePositions();
				wallShot.transform.SetParent(enemy);
				wallShot.SetActive(true);
				KWProjectiles.Add(wallShot);
			}
		}
		if (PlayerPrefs.GetInt("difficulty") >= 3) {
			for (int i = 0; i < 3; i++) {
				GameObject wallShot = EPPooler.GetComponent<ObjectPooler>().GetPool();
				wallShot.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);
				wallShot.transform.position = KWProjectilePositions();
				wallShot.transform.SetParent(enemy);
				wallShot.SetActive(true);
				KWProjectiles.Add(wallShot);
			}
		}
		if (M_Player.currentBG_name != killerWallBG.name) {
			foreach (GameObject p in KWProjectiles) {
				p.SetActive(false);
			}
			CancelInvoke();
		}
	}

	public Vector3 KBPositions() {
		killerblockpos = player.transform.position;
		while (Vector2.Distance(player.transform.position, killerblockpos) < 12) {

			float x = Random.Range(-killerblockBG.sizeDelta.x / 2 + scale, killerblockBG.sizeDelta.x / 2 - scale);
			float y = Random.Range(-killerblockBG.sizeDelta.y / 2 + scale, killerblockBG.sizeDelta.y / 2 - scale);
			killerblockpos = new Vector2(x, y);
		}
		return killerblockpos;
	}
	
	public Vector3 KWProjectilePositions() {
		return new Vector3(killerWallBG.position.x - 2 + killerWallBG.sizeDelta.x / 2, Random.Range(killerWallBG.position.y - killerWallBG.sizeDelta.y / 2, killerWallBG.position.y + killerWallBG.sizeDelta.y / 2), 0);
	}

	public void InvokeRepeatingScript(string name) {
		InvokeRepeating(name, 0.5f, 0.5f);
	}
}
