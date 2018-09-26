using System.Collections;
using System.Collections.Generic;

public class Playfield {

	public int width { get; protected set; }
	public int height { get; protected set; }

	Hex[,] hexes;
	public Character[,] characters { get; protected set; }

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

		characters = new Character[2,3];
		for (int player = 0; player < 2; player++) {
			for (int character = 0; character < 3; character++) {
				Character c = new Character(player);
				c.setPosition(getHexAt(charSpawns[3*player+character,0], charSpawns[3*player+character,1]));
				characters[player,character] = c;
			}
		}
	}

	public Hex getHexAt(int x, int y) {
		if ((x < 0 || y < 0) || (x >= width || y >= height)) {
			return null;
		}
		return hexes[x,y];
	}

	// TODO: Add proper goal tile handling. It's not walkable, but it's shootable.

	Hex getWalkableHexAt(int x, int y) {
		Hex hex = getHexAt(x, y);
		if (hex == null || !hex.isWalkable)
			return null;

		return hex;
	}

	public HashSet<Hex> getWalkableNeighboursFor(Hex hex, int distance) {
		if (distance <= 0) 
			return null;

		HashSet<Hex> neighbours = new HashSet<Hex>();
		neighbours = new HashSet<Hex>(getWalkableNeighboursFor(hex));

		while(distance > 1) {
			HashSet<Hex> neighboursIter = new HashSet<Hex>();

			foreach (var h in neighbours) {
				if (h == null)
					continue;

				neighboursIter.UnionWith(new HashSet<Hex>(getWalkableNeighboursFor(h)));
			}

			neighbours.UnionWith(neighboursIter);
			distance--;
		}

		neighbours.Remove(hex);

		return neighbours;
	}

	List<Hex> getWalkableNeighboursFor(Hex hex) {
		if (hex == null)
			return null;

		List<Hex> set = new List<Hex>();

		set.Add(getWalkableHexAt(hex.x-1, hex.y)); 			//left
		set.Add(getWalkableHexAt(hex.x+1, hex.y));			//right

		if (hex.y % 2 == 0) {
			set.Add(getWalkableHexAt(hex.x-1, hex.y+1));	// left up
			set.Add(getWalkableHexAt(hex.x, hex.y+1));		// right up
			set.Add(getWalkableHexAt(hex.x, hex.y-1));		// right down
			set.Add(getWalkableHexAt(hex.x-1, hex.y-1));	// left down
		} else {
			set.Add(getWalkableHexAt(hex.x, hex.y+1));		// left up
			set.Add(getWalkableHexAt(hex.x+1, hex.y+1));	// right up
			set.Add(getWalkableHexAt(hex.x+1, hex.y-1)); 	// right down
			set.Add(getWalkableHexAt(hex.x, hex.y-1));		// left down
		}

		return set;
	}

	public HashSet<Hex> getStraightNeighboursFor(Hex hex, int distance) {
		List<Hex> neighbours = getWalkableNeighboursFor(hex);

		List<Hex> neighboursIter = neighbours;
		while (distance > 1) {
			List<Hex> neighboursIter2 = new List<Hex>();
			for (int i = 0; i <= 5; i++) {
				neighboursIter2.Add(getStraightNeighbourInDirection(neighboursIter[i], i));
			}
			neighbours.AddRange(neighboursIter2);
			neighboursIter = neighboursIter2;

			distance--;
		}

		return new HashSet<Hex>(neighbours);
	}

	Hex getStraightNeighbourInDirection(Hex hex, int direction) {
		// 0 - left, 1 - right, 2 - left up, 3 - right up, 4 - right down, 5 - left down
		if (direction < 0 || direction > 5)
			return null;
				
		if (hex == null)
			return null;

		return getWalkableNeighboursFor(hex)[direction];
	}
}
