using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldController : MonoBehaviour {

	public static PlayfieldController Instance;

	public GameObject hexPrefab;

	Dictionary<Hex, GameObject> hexGOMap;
	Dictionary<GameObject, Hex> GOHexMap;

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
	}

	// Use this for initialization
	void Start() {
		drawHexes();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	public void drawHexes() {
		if (hexGOMap != null) {
			foreach (var hex in hexGOMap.Keys) {
				Destroy(hexGOMap[hex]);
			}
		}

		hexGOMap = new Dictionary<Hex, GameObject>();
		GOHexMap = new Dictionary<GameObject, Hex>();

		for (int x = 0; x < playfield.width; x++) {
			for (int y = 0; y < playfield.height; y++) {
				Hex hex = playfield.getHexAt(x, y);
				GameObject hexGO = Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				if (y % 2 > 0) {
					hexGO.transform.position = new Vector3(x + 0.5f, y * 0.75f, 0);
				} else {
					hexGO.transform.position = new Vector3(x, y * 0.75f, 0);
				}
				hexGO.transform.SetParent(this.transform);
				hexGO.name = "hex-" + x + "-" + y;
				hexGOMap.Add(hex, hexGO);
				GOHexMap.Add(hexGO, hex);
			}
		}
	}

	public Hex getHexFor(GameObject go) {
		return GOHexMap[go];
	}

	public GameObject getHexGOFor(Hex hex) {
		return hexGOMap[hex];
	}
}
