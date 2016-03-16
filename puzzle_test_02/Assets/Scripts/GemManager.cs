using UnityEngine;
using System.Collections;

public class GemManager : MonoBehaviour {

	public int gridWidth;
	public int gridHeight;
	public GameObject gemPrefab;
	
	// Use this for initialization
	void Start () {
		for (int y = 0; y < gridHeight; y++) {
			for (int x = 0; x < gridWidth; x++) {
				GameObject g = Instantiate(gemPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
				g.transform.parent = gameObject.transform;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
