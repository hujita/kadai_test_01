using UnityEngine;
using System.Collections;

public class SceneControl : MonoBehaviour {
	private BlockRoot block_root = null;

	// Use this for initialization
	void Start () {
		this.block_root = this.gameObject.GetComponent<BlockRoot> ();
		this.block_root.initialSetUp ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
