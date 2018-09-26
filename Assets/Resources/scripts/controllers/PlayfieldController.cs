using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldController : MonoBehaviour {

	public static PlayfieldController Instance;

	public GameObject ballPrefab;
	public GameObject characterPrefab;
	public GameObject hexPrefab;
	public Sprite hexNormal;
	public Sprite hexHighlight;
	public Sprite hexHighlightRed;
	public Sprite hexGoal;

	Dictionary<Hex, GameObject> hexGOMap;
	Dictionary<GameObject, Hex> GOHexMap;

	HashSet<Hex> availableHexesForAction;

	public int fieldWidth = 11;
	public int fieldHeight = 7;

	public Playfield playfield { get; protected set; }
	Character[,] characters;

	int currentPlayer;
	int[] score;

	void OnEnable() {
		if (Instance != null) {
			Debug.LogError("There should be only one PlayfieldController!");
		}
		Instance = this;

		playfield = new Playfield(fieldWidth, fieldHeight);
		hexGOMap = new Dictionary<Hex, GameObject>();
		GOHexMap = new Dictionary<GameObject, Hex>();
		availableHexesForAction = new HashSet<Hex>();
	}

	// Use this for initialization
	void Start() {
		drawHexes();
		spawnCharacters();
		spawnBall();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	void drawHexes() {
		if (hexGOMap.Count > 0) {
			foreach (var hex in hexGOMap.Keys) {
				Destroy(hexGOMap[hex]);
			}
		}

		hexGOMap = new Dictionary<Hex, GameObject>();
		GOHexMap = new Dictionary<GameObject, Hex>();

		for (int x = 0; x < playfield.width; x++) {
			for (int y = 0; y < playfield.height; y++) {
				Hex hex = playfield.getHexAt(x, y);
				if (hex == null)
					continue;
				
				GameObject hexGO = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				if (y % 2 > 0) {
					hexGO.transform.position = new Vector3(x + 0.5f, y * 0.75f, 0);
				} else {
					hexGO.transform.position = new Vector3(x, y * 0.75f, 0);
				}
				hexGO.transform.SetParent(this.transform);
				hexGO.name = "hex-" + x + "-" + y;

				if (hex.isGoal) {
					hexGO.GetComponent<SpriteRenderer>().sprite = hexGoal;
				}

				hexGOMap.Add(hex, hexGO);
				GOHexMap.Add(hexGO, hex);
			}
		}
	}

	public Hex getHexForGO(GameObject go) {
		return GOHexMap[go];
	}

	public GameObject getGOForHex(Hex hex) {
		return hexGOMap[hex];
	}

	public void getAvailableHexesForAction(string action, Hex origin, int distance) {
		if (availableHexesForAction.Count > 0) {
			removeHighlight();
		}

		switch (action) {
		case "move":
			availableHexesForAction = playfield.getWalkableNeighboursFor(origin, distance);
			break;
		case "kick":
			availableHexesForAction = playfield.getStraightNeighboursFor(origin, distance);
			break;
		case "tackle":
			break;
		default:
			return;
		}

		highlightAvailableHexes();
	}

	void highlightAvailableHexes() {
		foreach (var hex in availableHexesForAction) {
			if (hex == null)
				continue;

			if (hex.isWalkable) {
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexHighlight;	
			} else {
				if (hex.isGoal)
					// TODO: Add proper goal hex highlight
					continue;
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexHighlightRed;	
			}
		}
	}

	public void removeHighlight() {
		if (availableHexesForAction.Count == 0)
			return;

		foreach (var hex in availableHexesForAction) {
			if (hex == null)
				continue;
			
			hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexNormal;
		}

		availableHexesForAction = new HashSet<Hex>();
	}

	void spawnCharacters() {
		foreach (var c in playfield.characters) {
			Hex positionHex = c.position;
			GameObject hexGO = hexGOMap[positionHex];
			GameObject charGO = Instantiate(characterPrefab, hexGO.transform.position, Quaternion.identity);
			SpriteRenderer charSR = charGO.GetComponent<SpriteRenderer>();
			charSR.color = Color.yellow;
			if (c.player == 1) {
				charSR.color = Color.green;
				charSR.flipX = true;
			}

			charGO.transform.name = "char_" + c.player;
		}
	}

	void spawnBall() {
		// TODO: REDO
		Hex ballSpawn = playfield.getHexAt(playfield.ballSpawn[0], playfield.ballSpawn[1]);
		GameObject spawnGO = hexGOMap[ballSpawn];
		GameObject ballGO = Instantiate(ballPrefab, spawnGO.transform.position, Quaternion.identity);
		ballGO.transform.name = "ball";
		ballSpawn.setBall(true);
	}

}
