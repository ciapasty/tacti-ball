using System;

public class Ball {

	Hex _hex;
	public Hex hex { //get; protected set; }
		get {
			if (carriedBy != null)
				return carriedBy.hex;
			return _hex;
		} 
		protected set {
			_hex = value;
			if (cbBallMoved != null)
				cbBallMoved(this);
		}
	}
	public Character carriedBy { get; protected set; }

	Action<Ball> cbBallMoved;

	public Ball(Hex hex) {
		this.hex = hex;
		this.carriedBy = null;
	}

	public void pickedUpByCharacter(Character c) {
		carriedBy = c;
		c.hasBall = true;
		hex = null;
	}

	public void droppedOnHex(Hex hex) {
		carriedBy = null;
		this.hex = hex;
	}

	// ==== CALLBACKS ====

	public void registerBallMovedCallback(Action<Ball> callback) {
		cbBallMoved += callback;
	}

	public void unregisterBallMovedCallback(Action<Ball> callback) {
		cbBallMoved += callback;
	}

}
