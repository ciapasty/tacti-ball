using System.Collections;
using System.Collections.Generic;

public class Character {

	Hex position;

	public bool hasBall { get; protected set; }
	public int availableActions { get; protected set; }

	public Character() {
		
	}
}

