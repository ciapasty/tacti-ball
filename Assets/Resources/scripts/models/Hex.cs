using System.Collections;
using System.Collections.Generic;

public class Hex {

	public int x { get; protected set; }
	public int y { get; protected set; }

	public Hex(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public bool isWalkable = true;
	public bool hasBall = false;

}
