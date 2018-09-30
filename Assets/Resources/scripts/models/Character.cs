using System;
using System.Collections;
using System.Collections.Generic;

public class Character {

	public Hex hex { get; protected set; }
	public int player { get; protected set; }

	public bool hasBall { get; protected set; }
	public int availableActions { get; protected set; }

	Action<Character> cbPositionSet;

	public Character(int player, Hex startPosition) {
		this.player = player;
		hex = startPosition;
		hex.setWalkable(false);

		hasBall = false;
		availableActions = 2;
	}

	public void setPosition(Hex hex) {
		this.hex.setWalkable(true);
		this.hex = hex;

		if (cbPositionSet != null)
			cbPositionSet(this);

		hex.setWalkable(false);
	}

	public void setActions(int actions) {
		availableActions -= actions;
	}

	// ==== CALLBACKS ====

	public void registerPositionSetCallback(Action<Character> callback) {
		cbPositionSet += callback;
	}

	public void unregisterPositionSetCallback(Action<Character> callback) {
		cbPositionSet -= callback;
	}
}

