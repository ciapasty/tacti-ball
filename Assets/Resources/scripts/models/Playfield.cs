using System.Collections;
using System.Collections.Generic;

public class Playfield {

	Hex[,] hexes;

	public int width { get; protected set; }
	public int height { get; protected set; }

	public Playfield(int width, int height)	{
		this.width = width;
		this.height = height;

		hexes = new Hex[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				hexes[x,y] = new Hex(x, y);
			}
		}
	}

	public Hex getHexAt(int x, int y) {
		if ((x < 0 || y < 0) || (x >= width || y >= height)) {
			return null;
		}
		return hexes[x,y];
	}

	public HashSet<Hex> getNeighboursFor(Hex hex, int distance) {
		if (distance <= 0) { return null; }

		HashSet<Hex> neighbours = new HashSet<Hex>();
		neighbours = new HashSet<Hex>(getNeighboursFor(hex));

		while(distance > 1) {
			HashSet<Hex> neighboursIter = new HashSet<Hex>();

			foreach (var h in neighbours) {
				if (h == null) { continue; }
				neighboursIter.UnionWith(new HashSet<Hex>(getNeighboursFor(h)));
			}

			neighbours.UnionWith(neighboursIter);
			distance--;
		}

		neighbours.Remove(hex);

		return neighbours;
	}

	public List<Hex> getNeighboursFor(Hex hex) {
		if (hex == null) { return null; }

		List<Hex> set = new List<Hex>();

		set.Add(getHexAt(hex.x-1, hex.y)); 			//left
		set.Add(getHexAt(hex.x+1, hex.y));			//right

		if (hex.y % 2 == 0) {
			set.Add(getHexAt(hex.x-1, hex.y+1));	// left up
			set.Add(getHexAt(hex.x, hex.y+1));		// right up
			set.Add(getHexAt(hex.x, hex.y-1));		// right down
			set.Add(getHexAt(hex.x-1, hex.y-1));	// left down
		} else {
			set.Add(getHexAt(hex.x, hex.y+1));		// left up
			set.Add(getHexAt(hex.x+1, hex.y+1));	// right up
			set.Add(getHexAt(hex.x+1, hex.y-1)); 	// right down
			set.Add(getHexAt(hex.x, hex.y-1));		// left down
		}

		return set;
	}

	public HashSet<Hex> getStraightNeighboursFor(Hex hex, int distance) {
		List<Hex> neighbours = getNeighboursFor(hex);

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

	public Hex getStraightNeighbourInDirection(Hex hex, int direction) {
		// 0 - left, 1 - right, 2 - left up, 3 - right up, 4 - right down, 5 - left down
		if (direction < 0 || direction > 5) { return null; }
		if (hex == null) { return null; }

		return getNeighboursFor(hex)[direction];
	}
}
