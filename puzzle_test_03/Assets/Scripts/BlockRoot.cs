using UnityEngine;
using System.Collections;

public class BlockRoot : MonoBehaviour {
	public GameObject BlockPrefab = null;
	public BlockControl[,] blocks;

	private GameObject main_camera = null;
	private BlockControl grabbed_block = null;


	// Use this for initialization
	void Start () {
		this.main_camera = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mouse_position;
		this.unprojectMousePosition (out mouse_position, Input.mousePosition);
		Vector2 mouse_position_xy = new Vector2 (mouse_position.x, mouse_position.y);

		if (this.grabbed_block == null) {
			//if (!this.is_has_falling_block ()) {
				if (Input.GetMouseButtonDown (0)) {
					foreach (BlockControl block in this.blocks) {
						if (! block.isGrabbable ()) {
							continue;
						}

						if (!block.isContainedPosition(mouse_position_xy)) {
							continue;
						}

						this.grabbed_block = block;
						this.grabbed_block.beginGrab ();
						break;
					}
				}
			//}
		} else {
			if (! Input.GetMouseButton (0)) {
				this.grabbed_block.endGrab ();
				this.grabbed_block = null;
			}
		}
	}

	// Creating blocks and deploy
	public void initialSetUp() {
		this.blocks = new BlockControl [Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];

		int color_index = 0;

		for (int y = 0; y < Block.BLOCK_NUM_Y; y++) {
			for (int x = 0; x < Block.BLOCK_NUM_X; x++) {
				GameObject game_object = Instantiate (this.BlockPrefab) as GameObject;
				BlockControl block = game_object.GetComponent<BlockControl>();
				this.blocks[x, y] = block;

				block.i_pos.x = x;
				block.i_pos.y = y;

				block.block_root = this;

				Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
				block.transform.position = position;
				block.setColor ((Block.COLOR)color_index);

				block.name = "block(" + block.i_pos.x.ToString () + "," + block.i_pos.y.ToString() + ")";

				color_index = Random.Range (0, (int)Block.COLOR.NORMAL_COLOR_NUM);

			}
		}
	}

	public static Vector3 calcBlockPosition(Block.iPosition i_pos) {
		Vector3 position = new Vector3 (-(Block.BLOCK_NUM_X / 2.0f - 0.5f),
		                               -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

		position.x += (float)i_pos.x * Block.COLLISION_SIZE;
		position.y += (float)i_pos.y * Block.COLLISION_SIZE;

		return(position);
	}

	public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position) {
		bool ret;
		Plane plane = new Plane (Vector3.back, new Vector3 (0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));

		Ray ray = this.main_camera.GetComponent<Camera> ().ScreenPointToRay (mouse_position);
		float depth;

		if (plane.Raycast (ray, out depth)) {
			world_position = ray.origin + ray.direction * depth;
			ret = true;
		} else {
			world_position = Vector3.zero;
			ret = false;
		}

		return(ret);
	}
}
