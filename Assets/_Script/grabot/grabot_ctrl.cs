using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {

    public class grabot_ctrl : base_ctrl {
        [Header("   stand_up")]
		public float hand_zoom_help_deg = 10f;
		float _hand_zoom_help_cos = 0f;

		grabot_tracks _tracks = null;
        grabot_hand _hand = null;
		grabot_AI _AI = null;


		void Awake() {
			_tracks = GetComponentInChildren<grabot_tracks>();
            _hand = GetComponentInChildren<grabot_hand>();
			_hand.init(this);

			_hand_zoom_help_cos = Mathf.Cos(hand_zoom_help_deg * Mathf.Deg2Rad);


		}
		void Start() {
			body_relax();
		}

		public void init_AI (grabot_AI AI) {
			_AI = AI;
		}


        public override void use_item_start() {
            _hand.is_zoomed = !_hand.is_zoomed;
        }

        public override void body_relax() {
            _hand.relax();
        }
        public override void release_grab() {
            _hand.release_grab(); // отпустить
        }
        public override void enable_grab() {
            _hand.enable_grab();
        }
        public override bool is_grabbed() {
            return _hand.grabbed;
        }


        public override void body_jump_begin() {
			_tracks.move(_tracks.is_jumping ? Vector2.down : Vector2.up);
        }
        public override void body_jump_end() {
			_tracks.move(_tracks.is_jumping ? Vector2.up : Vector2.down);
        }



        public override void body_direct(Vector2 direction) {
            _hand.direct(direction);
            direction.y = 0;
            _tracks.move(direction);
        }

        public override void body_arm_full_direct(Vector2 direction) {
            _hand.direct(direction);
        }
        public override void body_leg_full_direct(Vector2 direction) {
            _tracks.move(direction);
        }
        
        public override void body_arm_group() {
            _hand.is_zoomed = ( _hand.is_folded || _hand.is_relaxed );
        }





        public void jump() {
            _tracks.move(Vector2.up);
            StartCoroutine(finish_jump());
        }
        IEnumerator finish_jump() {
            yield return new WaitForSeconds(1.5f);
            _tracks.move(Vector2.down);
        }



        public Vector2 base_pos {
            get { return _tracks.position; }
        }
        public Vector2 hand_pos {
            get { return _hand.position; }
        }
        public float rotation {
            get { return _tracks.rotation; }
        }

        public void hand_help2stand_up() {
			body_arm_full_direct(Vector2.down);
			_hand.is_zoomed = (hand_direction_cos(Vector2.down) > _hand_zoom_help_cos);
        }

        public bool is_hand_zoomed {
            get { return _hand.is_zoomed; }
            set { _hand.is_zoomed = value; }
        }
		public float hand_max_length {
			get { return _hand.max_length; }
		}
		public float hand_direction_cos(Vector2 direction) {
			return _hand.direction_cos(direction);
		}

		public void throw_object() {
			_hand.throw_object();
		}



		public Transform grab_target {
			get { return _AI == null ? null : _AI.grab_target; }
		}


        public override bool has_active_item() {
            return true;
        }

	}
}