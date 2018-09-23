using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
    public class als_item_magnet_belt : als_item {
		public int belt_layer;
		public GameObject halo;

		protected int _layer = 0;

		protected override void Start() {
			base.Start();

			_layer = gameObject.layer;
		}

        public override int type_id() {
            return 2;
        }

        public override bool use() {
			return _in_use ? do_normal() : do_magnet();
        }
        public override bool stop_use() {
			return true;
        }



		bool do_magnet() {
			if (!base.use())
				return false;
			gameObject.layer = belt_layer;
			halo.SetActive(true);
			_in_use = true;
			return true;
		}
        bool do_normal() {
			gameObject.layer = _layer;
			halo.SetActive(false);
			return base.stop_use();
		}

        

		/*
        void OnTriggerEnter2D(Collider2D other) {
            if (_others.IndexOf(other) < 0)
                _others.Add(other);
		}
        void OnTriggerExit2D(Collider2D other) {
            _others.Remove(other);
        }
        void OnTriggerStay2D(Collider2D other) {
            if (_others.Count == 0) return;

            Vector2 total_force = Vector2.zero;
            foreach (var c in _others) {
				Vector2 v = c.transform.position.vec2() - attach_body.position;
                float d = v.sqrMagnitude;
                if (d < utils.small_float) continue;
                total_force += v * (1f / d);
            }
            total_force *= 1f / (float)_others.Count;
            attach_body.AddForce(total_force * force, ForceMode2D.Impulse);
        }
*/



	}
}
