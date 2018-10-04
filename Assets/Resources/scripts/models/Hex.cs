using System;

public class Hex : ICloneable {

	public int x { get; protected set; }
	public int y { get; protected set; }

	public bool isWalkable { get; protected set; }
	public bool isGoal { get; protected set; }

//	public bool hasBall = false;

	public Hex(int x, int y) {
		this.x = x;
		this.y = y;
		isWalkable = true;
		isGoal = false;
	}

	Hex(int x, int y, bool isWalkable, bool isGoal) { //, bool hasBall) {
		this.x = x;
		this.y = y;
		this.isWalkable = isWalkable;
		this.isGoal = isGoal;
//		this.hasBall = hasBall;
	}

	public object Clone() {
		return new Hex(this.x, this.y, this.isWalkable, this.isGoal); //, this.hasBall);
	}

	public void setGoal(bool goal) {
		isGoal = goal;
		isWalkable = false;
	}

//	public void setBall(bool hasBall) {
//		this.hasBall = hasBall;
//	}

	public void setWalkable (bool walkable) {
		this.isWalkable = walkable;
	}

}
