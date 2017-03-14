using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour {

	public GameObject spikeBullet;
	public GameObject bomb;
	public GameObject player;
	public GameObject InfoAmmo;
	public Spike spike;
	public Vector2 mousepos;
	public ObjectPooler pool;
	public timer timer;

	public bool fireMode = false;
	public bool fireBullets = true;
	public bool visibleAlready = false;

	public static int bullets;
	public static int bombs;

	public Sprite spikeSprite;
	public Sprite bombSprite;

	private bool first = true;

	Color32 visible = new Color32(255, 255, 255, 255);

	Image bombGUI;
	public Text bombCount;

	Image bulletGUI;
	public Text bulletCount;

	Image currentAmmo;


	private void Start() {
		bombGUI = GameObject.Find("BombGUI").GetComponent<Image>();
		bulletGUI = GameObject.Find("BulletGUI").GetComponent<Image>();
		currentAmmo = GameObject.Find("CurrentAmmo").GetComponent<Image>();
		bullets = 0;
		bombs = 0;

	}

	void Update() {

		if (Input.GetKeyDown(KeyCode.Space)) {
			Cursor.visible = !Cursor.visible;
			fireMode = !fireMode;
			timer.attacking = !timer.attacking;

			if (M_Player.gameProgression != 10 && first) {
				if (bullets != 0) {
					Canvas_Renderer.script.infoRenderer("Wow, you figured out how to shoot ... ok.\n " +
														"Use your mouse to aim.\n "+
														"The bullets are reusable so pick them up after you fire!\n" +
														"Currently you have: " + bullets + " bullets.\n "+
														"Don't lose them", null);
					first = false;
				}else {
					Canvas_Renderer.script.infoRenderer("Wow, you figured out how to shoot ... ok.\n" +
														"Use your mouse to aim.\n "+
														"The bullets are reusable so pick them up after you fire!\n " +
														"Currently you have: " + bullets + " bullets.", null);
					first = false;
				}
			}
			visibleAlready = true;
			InfoAmmo.GetComponent<Text>().text = "Equiped:";
			bulletGUI.color = visible;
			currentAmmo.color = visible;
			bulletCount.text = "x " + bullets;
			bombGUI.color = visible;
			bombCount.text = "x " + bombs;
		}


		if (Input.GetMouseButtonDown(1)) {
			fireBullets = !fireBullets;
		}

		if (fireMode) {
			if (Input.GetMouseButtonDown(0) && fireBullets) {
				if (bullets >= 1) {
					print("Bullets remaining: " + (bullets - 1));
					FireSpike();
				}
				else {
					print("Out of bullets!");
				}
			}
			if(Input.GetMouseButtonDown(0) && !fireBullets) {
				if(bombs > 0) {
					FireBomb();
					StartCoroutine(RefreshBombs());
				}
				else {
					print("Out of bombs!");
				}
			}
		}
		if (fireBullets) {
			currentAmmo.sprite = spikeSprite;
		}
		else {
			currentAmmo.sprite = bombSprite;

		}
	}
	public void FireSpike() {

		Vector3 playpos = player.transform.position;
		mousepos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);

		GameObject bullet = pool.GetPool();


		bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, (Vector3)mousepos - player.transform.position);
		bullet.transform.position = player.transform.position - (bullet.transform.rotation * Vector2.down * 2);
		bullet.name = "Bullet";
		bullet.transform.parent = GameObject.Find("Collectibles").transform;
		bullet.SetActive(true);
		SoundFXHandler.script.PlayFX(SoundFXHandler.script.ArrowSound);

		bullets--;
		bulletCount.text = "x " + bullets;

	}

	public void FireBomb() {
		GameObject firedBomb = Instantiate(bomb);

		bombs--;
		BombScript script = firedBomb.GetComponent<BombScript>();
		Vector3 playpos = player.transform.position;

		firedBomb.transform.position = playpos + Vector3.down * 2.5f;
		firedBomb.name = "Bomb";

		script.primed = true;
		bombCount.text = "x " + bombs;
	}

	public IEnumerator RefreshBombs() {
		Canvas_Renderer.script.infoRenderer(null, "Wait for the bomb to regenerate!");
		yield return new WaitForSecondsRealtime(8);
		bombs++;
		bombCount.text = "x " + bombs;
		StopCoroutine(RefreshBombs());
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.name == "Spike1") {
			Destroy(col.gameObject);
			bullets++;
			bulletCount.text = "x " + bullets;
		}
		if (col.name == "BombPickup") {
			bombGUI.color = visible;
			bombCount.text = "x " + bombs;
		}
	}
}
