using UnityEngine;
using System.Collections;

public class Block {
	public static float COLLISION_SIZE = 1.0f;

	public struct iPosition {
		public int x;
		public int y;
	}

	public enum COLOR {
		NONE = -1,
		PINK = 0,
		BLUE,
		YELLOW,
		GREEN,
		MAGENTA,
		ORANGE,
		GRAY,
		NUM,
		FIRST = PINK,
		LAST = ORANGE,
		NORMAL_COLOR_NUM = GRAY,
	};

	public enum DIR4 {
		NONE = -1,
		RIGHT,
		LEFT,
		UP,
		DOWN,
		NUM,
	};

	public enum STEP {
		NONE = -1,
		IDLE = 0,
		GRABBED,
		RELEASED,
		SLIDE,
		VACANT,
		RESPAWN,
		FALL,
		LONG_SLIDE,
		NUM,
	};

	public static int BLOCK_NUM_X = 9;
	public static int BLOCK_NUM_Y = 9;
}

public class BlockControl : MonoBehaviour {
	public Block.COLOR color = (Block.COLOR)0;
	public BlockRoot block_root = null;
	public Block.iPosition i_pos;

	public Block.STEP step = Block.STEP.NONE;
	public Block.STEP next_step = Block.STEP.NONE;
	private Vector3 position_offset_initial = Vector3.zero;
	public Vector3 position_offset = Vector3.zero;

	public float vanish_timer = -1.0f;
	public Block.DIR4 slide_dir = Block.DIR4.NONE;
	public float step_timer = 0.0f;

	// Use this for initialization
	void Start () {
		this.setColor (this.color);
		this.next_step = Block.STEP.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mouse_position;
		this.block_root.unprojectMousePosition (out mouse_position, Input.mousePosition);
		Vector2 mouse_position_xy = new Vector2 (mouse_position.x, mouse_position.y);

		this.step_timer += Time.deltaTime;
		float slide_time = 0.2f;

		if (this.next_step == Block.STEP.NONE) {
			switch (this.step) {
			case Block.STEP.SLIDE:
				if (this.step_timer >= slide_time) {
					if (this.vanish_timer == 0.0f) {
						this.next_step = Block.STEP.VACANT;
					} else {
						this.next_step = Block.STEP.IDLE;
					}
				}
				break;
			}
		}

		while (this.next_step != Block.STEP.NONE) {
			this.step = this.next_step;
			this.next_step = Block.STEP.NONE;

			switch (this.step) {
			case Block.STEP.IDLE:
				this.position_offset = Vector3.zero;
				this.transform.localScale = Vector3.one * 1.0f;
				break;
			case Block.STEP.GRABBED:
				this.transform.localScale = Vector3.one * 1.2f;
				break;
			case Block.STEP.RELEASED:
				this.position_offset = Vector3.zero;
				this.transform.localScale = Vector3.one * 1.0f;
				break;
			case Block.STEP.VACANT:
				this.position_offset = Vector3.zero;
				break;
			}
			this.step_timer = 0.0f;
		}

		switch(this.step) {
		case Block.STEP.GRABBED:
			this.slide_dir = this.calcSlideDir (mouse_position_xy);
			break;
		case Block.STEP.SLIDE:
			float rate = this.step_timer / slide_time;
			rate = Mathf.Min (rate, 1.0f);
			rate = Mathf.Sin (rate*Mathf.PI / 2.0f);
			this.position_offset = Vector3.Lerp (this.position_offset_initial, Vector3.zero, rate);
			break;
		}
		Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

		this.transform.position = position;
	}

	public void setColor(Block.COLOR color) {
		this.color = color;
		Color color_value;

		switch (this.color) {
		default:
		case Block.COLOR.PINK:
			color_value = new Color(1.0f, 0.5f, 0.5f);
			break;
		case Block.COLOR.BLUE:
			color_value = Color.blue;
			break;
		case Block.COLOR.YELLOW:
			color_value = Color.yellow;
			break;
		case Block.COLOR.GREEN:
			color_value = Color.green;
			break;
		case Block.COLOR.MAGENTA:
			color_value = Color.magenta;
			break;
		case Block.COLOR.ORANGE:
			color_value = new Color(1.0f, 0.46f, 0.0f);
			break;
		}
		this.GetComponent<Renderer>().material.color = color_value;
	}

	public void beginGrab(){
		this.next_step = Block.STEP.GRABBED;
	}

	public void endGrab() {
		this.next_step = Block.STEP.IDLE;
	}

	public bool isGrabbable() {
		bool is_grabbable = false;

		switch (this.step) {
		case Block.STEP.IDLE:
			is_grabbable = true;
			break;
		}
		return(is_grabbable);
	}

	public bool isContainedPosition(Vector2 position) {
		bool ret = false;
		Vector3 center = this.transform.position;
		float h = Block.COLLISION_SIZE / 2.0f;

		do {
			if (position.x < center.x - h || center.x + h < position.x) {
				break;
			}
			if (position.y < center.y - h || center.y + h < position.y) {
				break;
			}

			ret = true;
		} while(false);

		return(ret);
	}


	public Block.DIR4 calcSlideDir(Vector2 mouse_position) {
		Block.DIR4 dir = Block.DIR4.NONE;
		Vector2 v = mouse_position - new Vector2 (this.transform.position.x, this.transform.position.y);

		if (v.magnitude > 0.1f) {
			if(v.y > v.x) {
				if (v.y > -v.x) {
					dir = Block.DIR4.UP;
				} else {
					dir = Block.DIR4.LEFT;
				}
			} else {
				if (v.y > -v.x) {
					dir = Block.DIR4.RIGHT;
				} else {
					dir = Block.DIR4.DOWN;
				}
			}
		}
		return(dir);
	}

	public float calcDirOffset(Vector2 position, Block.DIR4 dir)
	{
		float offset = 0.0f;
		Vector2 v = position - new Vector2 (this.transform.position.x, this.transform.position.y);
		switch (dir) {
		case Block.DIR4.RIGHT:
			offset = v.x;
			break;
		case Block.DIR4.LEFT:
			offset = -v.x;
			break;
		case Block.DIR4.UP:
			offset = v.y;
			break;
		case Block.DIR4.DOWN:
			offset = -v.y;
			break;
		}
		return(offset);
	}

	public void beginSlide(Vector3 offset)
	{
		this.position_offset_initial = offset;
		this.position_offset = this.position_offset_initial;

		this.next_step = Block.STEP.SLIDE;
	}
}
