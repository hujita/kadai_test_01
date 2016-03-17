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
			if (!this.is_has_falling_block ()) {
				if (Input.GetMouseButtonDown (0)) {
					foreach (BlockControl block in this.blocks) {
						if (! block.isGrabbable ()) {
							continue;
						}

						if (!block.isContainedPosition (mouse_position_xy)) {
							continue;
						}

						this.grabbed_block = block;
						this.grabbed_block.beginGrab ();
						break;
					}
				}
			}
		} else {
			do {
				BlockControl swap_target = this.getNextBlock (grabbed_block, grabbed_block.slide_dir);
				if (swap_target == null) {
					break;
				}
				if (! swap_target.isGrabbable ()) {
					break;
				}
				float offset = this.grabbed_block.calcDirOffset (mouse_position_xy, this.grabbed_block.slide_dir);
				if (offset < Block.COLLISION_SIZE / 2.0f) {
					break;
				}
				this.swapBlock (grabbed_block, grabbed_block.slide_dir, swap_target);
				this.grabbed_block = null;
			} while(false);

			if (! Input.GetMouseButton (0)) {
				this.grabbed_block.endGrab ();
				this.grabbed_block = null;
			}

			if (this.is_has_falling_block () || this.is_has_sliding_block ()) {
			} else {
				int ignite_count = 0;
				foreach (BlockControl  block in this.blocks) {
					if (! block.isIdle ()) {
						continue;
					}
					if (this.checkConnection (block)) {
						ignite_count++;
					}
				}

				if (ignite_count > 0) {
					int block_count = 0;
					foreach (BlockControl block in this.blocks) {
						if (block.isVanishing ()) {
							block.rewindVanishTimer ();
						}
					}
				}
			}
		}

		bool is_vanishing = this.is_has_vanishing_block ();
		do {
			if (is_vanishing) {
				break;
			}
			if (this.is_has_sliding_block ()) {
				break;
			}
			for (int x = 0; x < Block.BLOCK_NUM_X; x++) {
				if (this.is_has_sliding_block_in_column (x)) {
					continue;
				}
				for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++) {
					if (! this.blocks [x, y].isVacant ()) {
						continue;
					}
					for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++) {
						if (this.blocks [x, y1].isVacant ()) {
							continue;
						}
						this.fallBlock (this.blocks [x, y], Block.DIR4.UP,
						                this.blocks [x, y1]);
						break;
					}
				}
			}

			for (int x = 0; x < Block.BLOCK_NUM_X; x++) {
				int fall_start_y = Block.BLOCK_NUM_Y;
				for (int y = 0; y < Block.BLOCK_NUM_Y; y++) {
					if (! this.blocks [x, y].isVacant ()) {
						continue;
					}
					this.blocks [x, y].beginRespawn (fall_start_y);
					fall_start_y++;
				}
			}
		} while(false);
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

	public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
	{
		BlockControl next_block = null;

		switch (dir) {
		case Block.DIR4.RIGHT:
			if (block.i_pos.x < Block.BLOCK_NUM_X - 1) {
				next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
			}
			break;
		case Block.DIR4.LEFT:
			if (block.i_pos.x > 0) {
				next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
			}
			break;
		case Block.DIR4.UP:
			if (block.i_pos.y < Block.BLOCK_NUM_Y - 1) {
				next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
			}
			break;
		case Block.DIR4.DOWN:
			if (block.i_pos.y > 0) {
				next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
			}
			break;
		}
		return(next_block);
	}

	public static Vector3 getDirVector(Block.DIR4 dir)
	{
		Vector3 v = Vector3.zero;

		switch (dir) {
		case Block.DIR4.RIGHT:
			v = Vector3.right;
			break;
		case Block.DIR4.LEFT:
			v = Vector3.left;
			break;
		case Block.DIR4.UP:
			v = Vector3.up;
			break;
		case Block.DIR4.DOWN:
			v = Vector3.down;
			break;
		}

		v *= Block.COLLISION_SIZE;
		return(v);
	}

	public static Block.DIR4 getOppositDir(Block.DIR4 dir)
	{
		Block.DIR4 opposit = dir;
		switch (dir) {
		case Block.DIR4.RIGHT:
			opposit = Block.DIR4.LEFT;
			break;
		case Block.DIR4.LEFT:
			opposit = Block.DIR4.RIGHT;
			break;
		case Block.DIR4.UP:
			opposit = Block.DIR4.DOWN;
			break;
		case Block.DIR4.DOWN:
			opposit = Block.DIR4.UP;
			break;
		}
		return(opposit);
	}

	public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;

		Vector3 scale0 = block0.transform.localScale;
		Vector3 scale1 = block1.transform.localScale;

		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;

		Vector3 offset0 = BlockRoot.getDirVector (dir);
		Vector3 offset1 = BlockRoot.getDirVector (BlockRoot.getOppositDir (dir));

		block0.setColor (color1);
		block1.setColor (color0);

		block0.transform.localScale = scale1;
		block1.transform.localScale = scale0;

		block0.vanish_timer = vanish_timer1;
		block1.vanish_timer = vanish_timer0;

		block0.beginSlide (offset0);
		block1.beginSlide (offset1);
	}

	public bool checkConnection(BlockControl start)
	{
		bool ret = false;
		int normal_block_num = 0;

		if (! start.isVanishing ()) {
			normal_block_num = 1;
		}

		int rx = start.i_pos.x;
		int lx = start.i_pos.x;

		for (int x = lx - 1; x > 0; x--) {
			BlockControl next_block = this.blocks [x, start.i_pos.y];
			if (next_block.color != start.color) {
				break;
			}
			if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
				break;
			}
			if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
				break;
			}
			if (! next_block.isVanishing ()) {
				normal_block_num++;
			}
			lx = x;
		}

		for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++) {
			BlockControl next_block = this.blocks [x, start.i_pos.y];
			if (next_block.color != start.color) {
				break;
			}
			if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
				break;
			}
			if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
				break;
			}
			if (! next_block.isVanishing ()) {
				normal_block_num++;
			}
			rx = x;
		}

		do {
			if (rx - lx + 1 < 3) {
				break;
			}
			if (normal_block_num == 0) {
				break;
			}
			for (int x = lx; x < rx + 1; x++) {
				this.blocks [x, start.i_pos.y].toVanishing ();
				ret = true;
			}
		} while(false);

		normal_block_num = 0;
		if (! start.isVanishing ()) {
			normal_block_num = 1;
		}
		int uy = start.i_pos.y;
		int dy = start.i_pos.y;

		for (int y = dy - 1; y > 0; y--) {
			BlockControl next_block = this.blocks [start.i_pos.x, y];
			if (next_block.color != start.color) {
				break;
			}

			if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
				break;
			}
			if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
				break;
			}
			if (! next_block.isVanishing ()) {
				normal_block_num++;
			}
			dy = y;
		}

		for (int y = uy + 1; y > Block.BLOCK_NUM_Y; y++) {
			BlockControl next_block = this.blocks [start.i_pos.x, y];
			if (next_block.color != start.color) {
				break;
			}
			if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
				break;
			}
			if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
				break;
			}
			if (! next_block.isVanishing ()) {
				normal_block_num++;
			}
			uy = y;
		}

		do {
			if (uy - dy + 1 < 3) {
				break;
			}
			if (normal_block_num == 0) {
				break;
			}
			for (int y = dy; y < uy + 1; y++) {
				this.blocks [start.i_pos.x, y].toVanishing ();
				ret = true;
			}
		} while(false);
		return(ret);
	}

	private bool is_has_vanishing_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks) {
			if (block.vanish_timer > 0.0f) {
				ret = true;
				break;
			}
		}
		return(ret);
	}

	private bool is_has_sliding_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks) {
			if (block.step == Block.STEP.SLIDE) {
				ret = true;
				break;
			}
		}
		return(ret);
	}

	private bool is_has_falling_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks) {
			if (block.step == Block.STEP.FALL) {
				ret = true;
				break;
			}
		}
		return(ret);
	}

	public void fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;
		Vector3 scale0 = block0.transform.localScale;
		Vector3 scale1 = block1.transform.localScale;
		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;
		bool visible0 = block0.isVisible ();
		bool visible1 = block1.isVisible ();
		Block.STEP step0 = block0.step;
		Block.STEP step1 = block1.step;

		block0.setColor (color1);
		block1.setColor (color0);
		block0.transform.localScale = scale1;
		block1.transform.localScale = scale0;
		block0.vanish_timer = vanish_timer1;
		block1.vanish_timer = vanish_timer0;
		block0.setVisible (visible1);
		block1.setVisible (visible0);
		block0.step = step1;
		block1.step = step0;
		block0.beginFall (block1);
	}

	private bool is_has_sliding_block_in_column(int x)
	{
		bool ret = false;
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++) {
			if (this.blocks [x, y].isSliding ()) {
				ret = true;
				break;
			}
		}
		return(ret);
	}

}
