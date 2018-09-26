using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	PlayfieldController pfc;

	public Sprite hexHighlight;
	public int walkDistance = 2;
	public int kickDistance = 4;

	// Use this for initialization
	void Start () {
		pfc = FindObjectOfType<PlayfieldController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null) {
				Hex hex = pfc.getHexForGO(hit.collider.gameObject);
				pfc.getAvailableHexesForAction("move", hex, walkDistance);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null) {
				Hex hex = pfc.getHexForGO(hit.collider.gameObject);
				pfc.getAvailableHexesForAction("kick", hex, kickDistance);
			}
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			pfc.removeHighlight();
		}
	}
}
