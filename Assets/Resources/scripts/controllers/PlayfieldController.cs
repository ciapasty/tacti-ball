using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldController : MonoBehaviour {

	public GameObject ballPrefab;
	public GameObject characterPrefab;
	public GameObject hexPrefab;
	public Sprite hexNormal;
	public Sprite hexHighlight;
	public Sprite hexGoal;
	public Sprite hexGoalHighlight;

	Dictionary<Hex, GameObject> hexGOMap;
	Dictionary<GameObject, Hex> GOHexMap;
	public HashSet<Hex> highlightedHexes { get; protected set; }


	Dictionary<Character, GameObject> charGOMap;
	Dictionary<GameObject, Character> GOCharMap;

	Dictionary<Ball, GameObject> ballGOMap;

	public int fieldWidth = 11;
	public int fieldHeight = 7;

	public Playfield playfield { get; protected set; }

	int currentPlayer;
	int[] score;

	void OnEnable() {
		playfield = new Playfield(fieldWidth, fieldHeight);
		hexGOMap = new Dictionary<Hex, GameObject>();
		GOHexMap = new Dictionary<GameObject, Hex>();

		charGOMap = new Dictionary<Character, GameObject>();
		GOCharMap = new Dictionary<GameObject, Character>();

		ballGOMap = new Dictionary<Ball, GameObject>();

		playfield.registerCharacterMovedCallback(characterMoved);
		playfield.createCharacters();

		playfield.registerBallMovedCallback(ballMoved);
		playfield.createBall();
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

	// ==== Public methods ====

	public Hex getHexForGO(GameObject go) {
		return GOHexMap[go];
	}

	public GameObject getGOForHex(Hex hex) {
		return hexGOMap[hex];
	}

	public Character getCharacterForGO(GameObject go) {
		return GOCharMap[go];
	}

	public GameObject getGOForCharacter(Character c) {
		return charGOMap[c];
	}

	public void highlightHexes(HashSet<Hex> hexes) {
		if (highlightedHexes != null)
			removeHighlight();

		highlightedHexes = new HashSet<Hex>();
		foreach (var hex in hexes) {
			if (hex == null)
				continue;

			if (hex.isGoal) {
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexGoalHighlight;
			} else {
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexHighlight;	
			}
			highlightedHexes.Add(hex);
		}
	}

	public void removeHighlight() {
		if (highlightedHexes == null) {
			Debug.LogError("PlayfieldController.removeHighlight - ERROR: Tried to remove highlight with no hexes higlighted.");
			return;
		}

		foreach (var hex in highlightedHexes) {
			if (hex == null)
				continue;

			if (hex.isGoal) {
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexGoal;
			} else {
				hexGOMap[hex].GetComponent<SpriteRenderer>().sprite = hexNormal;
			}
		}
		highlightedHexes = null;
	}

	// ==== CALLBACKS ====

	void characterMoved(Character c) {
		GameObject charGO = charGOMap[c];
		GameObject hexGO = hexGOMap[c.hex];

		charGO.transform.position = hexGO.transform.position;
	}

	void ballMoved(Ball b) {
		GameObject ballGO = ballGOMap[b];
		GameObject hexGO = hexGOMap[b.hex];

		ballGO.transform.position = hexGO.transform.position;
	}

	// ==== Unitility methods ====

	void spawnCharacters() {
		foreach (var c in playfield.characters) {
			Hex positionHex = c.hex;
			GameObject hexGO = hexGOMap[positionHex];
			GameObject charGO = Instantiate(characterPrefab, hexGO.transform.position, Quaternion.identity);
			SpriteRenderer charSR = charGO.GetComponent<SpriteRenderer>();
			charSR.color = Color.yellow;
			if (c.player == 1) {
				charSR.color = Color.green;
				charSR.flipX = true;
			}

			charGO.transform.name = "char_" + c.player;

			charGOMap.Add(c, charGO);
			GOCharMap.Add(charGO, c);
		}
	}

	// TODO: redo properly
	public void updateCharacterActionsUI(Character c) {
		getGOForCharacter(c).GetComponentInChildren<UnityEngine.UI.Text>().text = c.availableActions.ToString();
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

	void spawnBall() {
		GameObject spawnGO = hexGOMap[playfield.ball.hex];
		GameObject ballGO = Instantiate(ballPrefab, spawnGO.transform.position, Quaternion.identity);
		ballGO.transform.name = "ball";
		ballGOMap.Add(playfield.ball, ballGO);
	}

}
