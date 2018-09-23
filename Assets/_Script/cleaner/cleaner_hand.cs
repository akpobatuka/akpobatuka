using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AssemblyCSharp {
	public class cleaner_hand : MonoBehaviour {
		public Rigidbody2D rb_gun; // пушка
		public Rigidbody2D rb_mid; // соединение посередине
		public HingeJoint2D base_joint; // 

		public float move_speed = 1f;
		public float rotate_speed = 1f;
		
		public float move_delta = 5f;
		public float rotate_delta = 5f;
		
		public GameObject bomb_prefab;

		public float bend_rate = .01f;
		public float bend_swap_y = 50f;


		Vector2 _gun_target_dir;
		Vector2 _gun_target_pos;
		Vector2 _mid_target_pos;
		

		bool _move_done = true;
		bool _rotate_done = true;
		
		List<air_bomb> _bombs = new List<air_bomb>(3);
		float _aaa = float.MaxValue;
		Vector2 _gun_prev;
		Vector2 _mid_prev;
		
		
		// initialization
		void Start () {
			/*
			for (int i = 0; i < 3; ++i)
				_bombs.Add(utils.resource_instantiate("air_bomb").GetComponent<air_bomb>());
			
			foreach (var b in _bombs)
				b.hide();
			*/
			gun_pos(gun_position);
			gun_direct(gun_direction);
		}


		public void gun_pos(Vector2 position) {
			_move_done = false;	
			_gun_target_pos = position;

			var d = position - base_joint.connectedAnchor;
			d *= .5f;
			var p = d.normalized.perp();


			if ((position.y < bend_swap_y && p.y > 0f) ||
				(position.y > bend_swap_y && p.y < 0f))
				p *= -1f;
			
			_mid_target_pos = p * bend_rate / d.q_dist() + base_joint.connectedAnchor + d;
			//p * Mathf.Sqrt(_len_avg_sq - d.sqrMagnitude) + position + d;

			_gun_prev = position - gun_position;
			_mid_prev = _mid_target_pos - rb_mid.position;
		}
		
		public void gun_direct(Vector2 direction) {
			_rotate_done = false;
			_gun_target_dir = direction;
		}
		
		public void fire() {
			foreach (var b in _bombs) {
				if (b.is_active) {
					b.fire(rb_gun.position, gun_direction);
					break;
				}
			}
		}
		
		public Vector2 gun_direction {
			get { return Vector2.up.Rotate(rb_gun.rotation); }
		}
		public Vector2 gun_position {
			get { return rb_gun.position; }
		}

	
		// FixedUpdate
		void FixedUpdate () {
			// move
			if (!_move_done) {
				//step_move(rb_mid, _mid_target_pos, ref _mid_prev); // mid pos
				_move_done = step_move(rb_gun, _gun_target_pos, ref _gun_prev); // gun pos
			}


			// rotate
			if (!_rotate_done) {
				rb_gun.clean_rotation();
				float a = gun_direction.get_angle_to(_gun_target_dir);
				float b = Mathf.Abs(a);
				if (b < rotate_delta && _aaa < b) {
					rb_gun.rotation -= a;
					rb_gun.angularVelocity = 0f;
					_aaa = float.MaxValue;
					_rotate_done = true;
				} else {
					rb_gun.angularVelocity = -a * rotate_speed;
					//rb_gun.AddTorque(-a * rotate_speed, ForceMode2D.Impulse);
					_aaa = b;
				}
			}
		}
		
		bool step_move(Rigidbody2D rb, Vector2 pos, ref Vector2 prev) {
			var q = pos - rb.position;
			float e = Vector2.Dot(q, prev);
			if (e < 0f) {
				prev = q;
				if (q.q_dist() < move_delta) {
					rb.position = pos;
					rb.velocity = Vector2.zero;
					return true;
				}
			}
			rb.velocity = q.normalized * move_speed;
			//rb.AddForce(q.normalized * move_speed, ForceMode2D.Impulse);

			return false;
		}
		/*
		void hard_rotate(Transform xf, Transform xf_target) {
			float a = xf.TransformDirection(Vector2.right).vec2().get_angle_to(xf_target.position.vec2() - xf.position.vec2());
			xf.rotateZ(a);
		}
*/

		
		public bool is_xform_done {
			get { return _move_done && _rotate_done; }
		}


		
	}

}