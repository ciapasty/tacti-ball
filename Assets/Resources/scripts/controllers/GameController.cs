using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {

	public static GameController Instance;

	public GameObject charActionsMenuPrefab;
	GameObject charActionsMenu;

	PlayfieldController pfc;

	enum GameState { SwitchTurn, PlayerStart, CharacterSelect, CharacterAction };
	GameState gameState;

	int activePlayer;	// 0 - player 1; 1 - player 2
	List<Character> playerCharacters;
	Character selectedCharacter;
	string selectedAction;

	void OnEnable() {
		if (Instance != null) {
			Debug.LogError("There should be only one GameController!");
		}
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		pfc = FindObjectOfType<PlayfieldController>();

		activePlayer = 0;
		gameState = GameState.PlayerStart;
	}

	// Update is called once per frame
	void Update () {
		// Reset state - change turns
		if (gameState == GameState.SwitchTurn) {
			activePlayer = (activePlayer == 0) ? 1 : 0;
			foreach (var c in pfc.playfield.characters) {
				if (c.player == activePlayer) {
					c.availableActions = (c.hasBall == false) ? 2 : 1;
					pfc.updateCharacterActionsUI(c);
				}
			}
			gameState = GameState.PlayerStart;
		}

		// Player start state
		// - Prepare characters, field, etc. for current player
		// - move to Select Character state
		if (gameState == GameState.PlayerStart) {
			playerCharacters = pfc.playfield.getCharactersWithActionsForPlayer(activePlayer);
			if (playerCharacters.Count == 0) {
				gameState = GameState.SwitchTurn;
				return;
			}
			highlightCharacterHexesForPlayer(activePlayer);
			gameState = GameState.CharacterSelect;
		}
			
		// Select Character state 
		// DONE - handle Action Selection UI, cancelling/selecting different character
		// DONE - highlight available characters hexes
		// DONE - move to Character Action (with selected action)
		if (gameState == GameState.CharacterSelect) {
			if (Input.GetMouseButtonUp(0)) {
				if (EventSystem.current.IsPointerOverGameObject())
				{
					//Debug.Log("Clicked on the UI");
					return;
				}
	
				RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				foreach (var hit in hits) {
					if (hit.collider != null && hit.collider.gameObject.tag == "Character") {
						Character c = pfc.getCharacterForGO(hit.collider.gameObject);
						if (playerCharacters.Contains(c)) {
							selectedCharacter = c;
							createActionsMenu(hit.collider.gameObject.transform.position);
							return;
						}
					}
				}

				Destroy(charActionsMenu);
				selectedCharacter = null;
			}
				
			if (Input.GetMouseButtonUp(1)) {
				if (charActionsMenu != null)
					Destroy(charActionsMenu);

				if (selectedCharacter != null)
					selectedCharacter = null;
			}
		}

		// Character Action state
		// - Highlight available hexes
		// - Perform action
		// - Check available actions for character -> Return to Select Character (with blocked selection)
		// - Check available characters -> Return to Select Character (with no Character selected)
		// - Switch Player -> return to Player start state
		if (gameState == GameState.CharacterAction) {
			if (Input.GetMouseButtonUp(0)) {
				if (EventSystem.current.IsPointerOverGameObject())
				{
					//Debug.Log("Clicked on the UI");
					return;
				}

				RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				foreach (var hit in hits) {
					if (hit.collider != null && hit.collider.gameObject.tag == "Hex") {
						Hex hex = pfc.getHexForGO(hit.collider.gameObject);
						if(pfc.highlightedHexes.Contains(hex)) {
							switch (selectedAction) {
							case "move":
//								selectedCharacter.availableActions -= 1;
//								selectedCharacter.moveToHex(hex);
								selectedCharacter.performAction(pfc.playfield.moveCharacterAction, hex);
								break;
							case "kick":
								selectedCharacter.performAction(pfc.playfield.kickBallAction, hex);
								break;
							case "tackle":
								selectedCharacter.performAction(pfc.playfield.tackleAction, hex);
								break;
							default:
								break;
							}

							pfc.removeHighlight();
							pfc.updateCharacterActionsUI(selectedCharacter);
							gameState = GameState.PlayerStart;
						}
					}
				}
			}


			if (Input.GetMouseButtonUp(1)) {
				gameState = GameState.PlayerStart;
			}
		}
	}

	void finishTurn() {
		gameState = GameState.SwitchTurn;
	}

	void highlightCharacterHexesForPlayer(int player) {
		HashSet<Hex> charHexes = new HashSet<Hex>();
		foreach (var c in playerCharacters) {
			charHexes.Add(c.hex);
		}
		pfc.highlightHexes(charHexes);
	}

	void selectAction(string action) {
		selectedAction = action;
		gameState = GameState.CharacterAction;
		Destroy(charActionsMenu);

		// TODO: highlight hexes available for action
		HashSet<Hex> availableHexes = pfc.playfield.getAvailableHexesForAction(selectedAction, selectedCharacter.hex);
		if (availableHexes == null) {
			Debug.Log("GameController.selectAction - ERROR: getAvailableHexesForAction returned null. Might be invoked with unsupported action");
			return;
		}

		pfc.highlightHexes(availableHexes);
	}

	void createActionsMenu(Vector3 position) {
		if (charActionsMenu != null)
			Destroy(charActionsMenu);
		
		charActionsMenu = Instantiate(charActionsMenuPrefab, position, Quaternion.identity);

		// Move button
		UnityEngine.UI.Button butt = charActionsMenu.transform.Find("MoveButton").gameObject.GetComponent<UnityEngine.UI.Button>();
		butt.onClick.AddListener(delegate {selectAction("move"); });

		// Kick button
		butt = charActionsMenu.transform.Find("KickButton").gameObject.GetComponent<UnityEngine.UI.Button>();
		if (selectedCharacter.hasBall) {
			butt.onClick.AddListener(delegate {selectAction("kick"); });
		} else {
			// TODO: gray out
		}

		// Tackle/takover button
		butt = charActionsMenu.transform.Find("TackleButton").gameObject.GetComponent<UnityEngine.UI.Button>();
		if (pfc.playfield.getNeighbourWithBall(selectedCharacter.hex) != null) {
			butt.onClick.AddListener(delegate {selectAction("tackle"); });
		} else {
			// TODO: gray out
		}
	}

}
