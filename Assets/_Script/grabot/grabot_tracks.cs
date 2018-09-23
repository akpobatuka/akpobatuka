using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class grabot_tracks : MonoBehaviour {
        [Header("phys")]
        public float track_force = 10f;
		public float jump_force = 5f;
		public float ground_contact_delta_time = 1f;

        [Header("graph")]
        public Vector2 tex_offset_speed;
        public SkinnedMeshRenderer tracks_mesh_renderer;
		public int material_id;
        Material _tracks_mat;


        SliderJoint2D _slider = null;
        Rigidbody2D _rbody = null;
		grabot_tracks_collider _coll = null;

        void Awake() {
            _slider = GetComponent<SliderJoint2D>();
            _rbody = GetComponent<Rigidbody2D>();
			_coll = GetComponentInChildren<grabot_tracks_collider>();

            _tracks_mat = tracks_mesh_renderer.materials[material_id];
        }


		public void relax() {
			_slider.useMotor = false;
		}


        public void move(Vector2 cross) {
            // движение гусениц
			_tracks_mat.add_main_tex_offset(tex_offset_speed * cross.x); // смещение текстуры гусениц

			if (_coll.contact_time + ground_contact_delta_time > Time.time) {
				var dir = _coll.contact_normal.perp();

				if (cross.x < -utils.small_float)
					goto force;
				if (cross.x > utils.small_float) {
					dir *= -1f;
					goto force;
				}
				goto endmove;		

            	force:
				_rbody.AddForceAtPosition(dir * track_force, _coll.contact_point);
			}
			endmove:

            // прыжки
            var m = _slider.motor;
			m.motorSpeed = cross.y * jump_force;
            _slider.motor = m;
			_slider.useMotor = true;

        }

		public Vector2 position {
			get { return _rbody.position; }
		}
        public float rotation {
            get { return _rbody.rotation; }
        }


		public bool is_jumping {
			get { return _slider.motor.motorSpeed > 0f; }
		}
        

	}
}
