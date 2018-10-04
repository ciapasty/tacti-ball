using System;
using System.Collections.Generic;

public class Character {

	Hex _hex;
	public Hex hex { 
		get {
			return _hex;
		} 
		set {
			_hex = value;
			if (cbCharacterMoved != null)
				cbCharacterMoved(this);
		}
	}
	public int player { get; protected set; }

	public bool hasBall = false;
//	public Ball ball { get; protected set; }
	public int availableActions;
//	Queue<Action<Character, Hex>> actions;

	Action<Character> cbCharacterMoved;

	public Character(int player, Hex startPosition) {
		this.player = player;
		hex = startPosition;
		hex.setWalkable(false);

		// TODO: redo to make more universal. Created on start of the turn
		availableActions = 2;
//		actions = new Queue<Action<Character, Hex>>(availableActions);
	}

	public void performAction(Action<Character, Hex> action, Hex hex) {
		action(this, hex);
		availableActions -= 1;
	}

	// ==== CALLBACKS ====

	public void registerCharacterMovedCallback(Action<Character> callback) {
		cbCharacterMoved += callback;
	}

	public void unregisterCharacterMovedCallback(Action<Character> callback) {
		cbCharacterMoved -= callback;
	}


}
