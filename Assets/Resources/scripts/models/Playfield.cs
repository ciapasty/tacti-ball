using System;
using System.Collections;
using System.Collections.Generic;

public class Playfield {

	public int width { get; protected set; }
	public int height { get; protected set; }

	Hex[,] hexes;
	public Character[,] characters { get; protected set; }

	Action<Character> cbCharPositionSet;

	// int[x,y] -> 0-2 Player1, 3-5 Player2
	public int[,] charSpawns = new int[,] { {3, 1}, {2, 3}, {3, 5}, {7, 1}, {8, 3}, {7, 5} };
	public int[] ballSpawn = new int[] {5, 3};

	public Playfield(int width, int height)	{
		this.width = width;
		this.height = height;

		hexes = new Hex[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				// Exclude hexes to form playfield
				if (x == 0 && y == 0 ||
				    x == 1 && y == 0 ||
				    x == 0 && y == 1 ||
				    x == 0 && y == 2 ||
				    x == 0 && y == 4 ||
				    x == 0 && y == 5 ||
				    x == 0 && y == 6 ||
					x == 1 && y == 6 ||
					x == 10 && y == 0 ||
					x == 10 && y == 1 ||
					x == 10 && y == 6 ||
					x == 10 && y == 5) 
				{
					hexes[x,y] = null;
				} else {
					hexes[x,y] = new Hex(x,y);
				}
				// Set these hexes as goals
				if (x == 0 && y == 3 ||
					x == 10 && y == 3) {
					hexes[x,y].setGoal(true);
				}
			}
		}
	}

	// ==== Public methods ====

	public Hex getHexAt(int x, int y) {
		if ((x < 0 || y < 0) || (x >= width || y >= height)) {
			return null;
		}
		return hexes[x,y];
	}

	public HashSet<Hex> getAvailableHexesForAction(string action, Hex origin) {
		switch (action) {
		case "move":
			return getWalkableNeighboursFor(origin, 1);
		case "kick":
			return getStraightNeighboursFor(origin, 4);
		case "tackle":
			Hex hex = getNeighbourWithBall(origin);
			if (hex != null) {
				HashSet<Hex> set = new HashSet<Hex>();
				set.Add(hex);
				return set;
			}
			return null;
		default:
			return null;
		}
	}

	public List<Character> getCharactersWithActionsForPlayer(int player) {
		List<Character> charsWithActions = new List<Character>();
		for (int i = 0; i < 3; i++) {
			if (characters[player,i].availableActions > 0)
				charsWithActions.Add(characters[player,i]);
		}
		return charsWithActions;
	}

	public Hex getNeighbourWithBall(Hex hex) {
		List<Hex> neighbours = getNeighboursFor(hex, false);
		foreach (var c in characters) {
			if (neighbours.Contains(c.hex) && c.hasBall)
				return c.hex;
		}
		return null;
	}

	public void createCharacters() {
		characters = new Character[2,3];
		for (int player = 0; player < 2; player++) {
			for (int character = 0; character < 3; character++) {
				Character c = new Character(player, getHexAt(charSpawns[3*player+character,0], charSpawns[3*player+character,1]));
				c.registerPositionSetCallback(cbCharPositionSet);
				characters[player,character] = c;
			}
		}
	}

	// ==== CALLBACKS ====

	public void registerCharacterPositionSetCallback(Action<Character> callback) {
		cbCharPositionSet += callback;
	}

	public void unregisterCharacterPositionSetCallback(Action<Character> callback) {
		cbCharPositionSet += callback;
	}

	// ==== Utility methods ====

	void characterPositionSet(Character c) {
		if (cbCharPositionSet != null)
			cbCharPositionSet(c);
	}
		
	Hex getWalkableHexAt(int x, int y) {
		Hex hex = getHexAt(x, y);
		if (hex == null || !hex.isWalkable)
			return null;

		return hex;
	}

	HashSet<Hex> getWalkableNeighboursFor(Hex hex, int distance) {
		if (distance <= 0) 
			return null;

		HashSet<Hex> neighbours = new HashSet<Hex>();
		neighbours = new HashSet<Hex>(getNeighboursFor(hex, true));

		while(distance > 1) {
			HashSet<Hex> neighboursIter = new HashSet<Hex>();

			foreach (var h in neighbours) {
				if (h == null)
					continue;

				neighboursIter.UnionWith(new HashSet<Hex>(getNeighboursFor(h, true)));
			}

			neighbours.UnionWith(neighboursIter);
			distance--;
		}

		neighbours.Remove(hex);

		return neighbours;
	}

	List<Hex> getNeighboursFor(Hex hex, bool walkable) {
		if (hex == null)
			return null;

		List<Hex> set = new List<Hex>();

		set.Add(walkable ? getWalkableHexAt(hex.x-1, hex.y) : getHexAt(hex.x-1, hex.y)); 			//left
		set.Add(walkable ? getWalkableHexAt(hex.x+1, hex.y) : getHexAt(hex.x+1, hex.y));			//right

		if (hex.y % 2 == 0) {
			set.Add(walkable ? getWalkableHexAt(hex.x-1, hex.y+1) : getHexAt(hex.x-1, hex.y+1));	// left up
			set.Add(walkable ? getWalkableHexAt(hex.x, hex.y+1) : getHexAt(hex.x, hex.y+1));		// right up
			set.Add(walkable ? getWalkableHexAt(hex.x, hex.y-1) : getHexAt(hex.x, hex.y-1));		// right down
			set.Add(walkable ? getWalkableHexAt(hex.x-1, hex.y-1) : getHexAt(hex.x-1, hex.y-1));	// left down
		} else {
			set.Add(walkable ? getWalkableHexAt(hex.x, hex.y+1) : getHexAt(hex.x, hex.y+1));		// left up
			set.Add(walkable ? getWalkableHexAt(hex.x+1, hex.y+1) : getHexAt(hex.x+1, hex.y+1));	// right up
			set.Add(walkable ? getWalkableHexAt(hex.x+1, hex.y-1) : getHexAt(hex.x+1, hex.y-1)); 	// right down
			set.Add(walkable ? getWalkableHexAt(hex.x, hex.y-1) : getHexAt(hex.x, hex.y-1));		// left down
		}

		return set;
	}

	HashSet<Hex> getStraightNeighboursFor(Hex hex, int distance) {
		List<Hex> neighbours = getNeighboursFor(hex, false);

		List<Hex> neighboursIter = neighbours;
		while (distance > 1) {
			List<Hex> neighboursIter2 = new List<Hex>();
			for (int i = 0; i <= 5; i++) {
				neighboursIter2.Add(getNeighbourInDirection(neighboursIter[i], i));
			}
			neighbours.AddRange(neighboursIter2);
			neighboursIter = neighboursIter2;

			distance--;
		}

		neighbours.RemoveRange(0, 6);

		return new HashSet<Hex>(neighbours);
	}

	Hex getNeighbourInDirection(Hex hex, int direction) {
		// 0 - left, 1 - right, 2 - left up, 3 - right up, 4 - right down, 5 - left down
		if (direction < 0 || direction > 5)
			return null;
				
		if (hex == null)
			return null;

		if (isCharacterOnHex(hex))
			return null;

		return getNeighboursFor(hex, false)[direction];
	}

	bool isCharacterOnHex(Hex hex) {
		foreach (var c in characters) {
			if (c.hex == hex)
				return true;
		}
		return false;
	}
}
