using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_item_slomo : als_item {
        [Header("slomo")]
        public float slow_time = 5f; // время замедления в секундах
        public float time_scale = .5f; // к-т замедления
		public float magic_force_magnifier = 2f; // уселитель волшебной силы

		als_camera_movement _camera = null;

		protected override void Start() {
			base.Start();
			_camera = FindObjectOfType<als_camera_movement>();
		}


        public override bool use() {
			return _in_use ? do_normal() : do_slow();
        }
        public override bool stop_use() {
			return true;
        }

        IEnumerator waitfor() {
			yield return new WaitForSeconds(slow_time);
			do_normal();
		}


		bool do_slow() {
			if (!base.use())
				return false;

			Time.timeScale = time_scale;
			_camera.slomo();
			_in_use = true;
			_pawn.set_magic_magnifier(magic_force_magnifier);
			StartCoroutine(waitfor());
			return true;
		}
		bool do_normal() {
			Time.timeScale = 1f;
			_camera.standart();
			_pawn.set_magic_magnifier(1f);
			return base.stop_use();
		}

        public override int type_id() {
            return 1;
        }

	}
}
