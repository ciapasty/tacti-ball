using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	PlayfieldController pfc;

	public Sprite hexHighlight;
	public int distance = 1;

	// Use this for initialization
	void Start () {
		pfc = FindObjectOfType<PlayfieldController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null) {
				Hex hex = pfc.getHexFor(hit.collider.gameObject);
				HashSet<Hex> hexNb = pfc.playfield.getNeighboursFor(hex, distance);

				foreach (var h in hexNb) {
					if (h == null) { continue; }

					GameObject hexGO = pfc.getHexGOFor(h);
					hexGO.GetComponent<SpriteRenderer>().sprite = hexHighlight;
				}

				Debug.Log ("Target Position: " + hit.collider.gameObject.transform.name);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null) {
				Hex hex = pfc.getHexFor(hit.collider.gameObject);
				HashSet<Hex> hexNb = pfc.playfield.getStraightNeighboursFor(hex, distance);

				foreach (var h in hexNb) {
					if (h == null) { continue; }

					GameObject hexGO = pfc.getHexGOFor(h);
					hexGO.GetComponent<SpriteRenderer>().sprite = hexHighlight;
				}

				Debug.Log ("Target Position: " + hit.collider.gameObject.transform.name);
			}
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			pfc.drawHexes();
		}
	}
}
