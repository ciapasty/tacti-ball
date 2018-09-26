using System.Collections;
using System.Collections.Generic;

public class Character {

	public Hex position { get; protected set; }
	// crate callback for position set

	public int player { get; protected set; }

	public bool hasBall { get; protected set; }
	public int availableActions { get; protected set; }

	public Character(int player) {
		this.player = player;
		hasBall = false;
		availableActions = 2;
	}

	public void setPosition(Hex hex) {
		position = hex;
	}
}

