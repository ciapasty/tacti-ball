using System.Collections;
using System.Collections.Generic;

public class Hex {

	public int x { get; protected set; }
	public int y { get; protected set; }

	public bool isWalkable { get; protected set; }
	public bool isGoal { get; protected set; }

	public bool hasBall = false;


	public Hex(int x, int y) {
		this.x = x;
		this.y = y;
		isWalkable = true;
		isGoal = false;
	}

	public void setGoal(bool goal) {
		isGoal = goal;
		isWalkable = false;
	}

	public void setBall(bool hasBall) {
		this.hasBall = hasBall;
	}

	public void setWalkable (bool walkable) {
		this.isWalkable = walkable;
	}

}
