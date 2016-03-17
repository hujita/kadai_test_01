using UnityEngine;
using System.Collections;

public class BlockRoot : MonoBehaviour {
	public GameObject BlockPrefab = null;
	public BlockControl[,] blocks;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

}
