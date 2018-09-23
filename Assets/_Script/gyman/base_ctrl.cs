using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class base_ctrl : behaviour_fixed_update {
        public als_game_control input_control { get; set; }

        public virtual void use_item_start() { }
        public virtual void use_item_stop() { }
        public virtual void body_relax() { }


        public virtual void release_grab() { }
        public virtual void enable_grab() { }

        public virtual bool is_grabbed() {
            return false;
        }

		public virtual bool is_arm_grouped() {
			return false;
		}
		public virtual bool is_leg_grouped() {
			return false;
		}
		public virtual bool is_jumping() {
			return false;
		}


        public virtual void body_jump_begin() { }
        public virtual void body_jump_end() { }



        public virtual void body_direct(Vector2 direction) { }

        public virtual void change_controls_roll_dir() { }

        public virtual void body_arm_full_direct(Vector2 direction) { }
        public virtual void body_arm_group() { }
        public virtual void body_leg_full_direct(Vector2 direction) { }
        public virtual void body_leg_group() { }



        public virtual bool has_active_item() {
            return false;
        }

		public virtual void assing_active_item(als_item item) {
			if (item != null)
				item.set_active_pawn(this);
		}
		public virtual als_item active_item() {
			return null;
		}

		public virtual void set_magic_magnifier(float m) {
		}

    }
}