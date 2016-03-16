using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {

	public GameObject sphere;
	public string ballName = "";
	
	string[] ballMats = {"ball_01", "ball_02", "ball_03", "ball_04", "ball_05", "ball_06"};

	public void CreateGem() {
		ballName = ballMats[Random.Range(0, ballMats.Length)];
		Material ballMat = Resources.Load ("Materials/" + ballName) as Material;
		sphere.GetComponent<Renderer>().material = ballMat;
	}

	// Use this for initialization
	void Start () {
		CreateGem ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
