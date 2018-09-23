using UnityEngine;
using System;


namespace AssemblyCSharp {
    public class als_item : MonoBehaviour, IComparable<als_item> {
        [Header("base")]
        public int max_capacity = 5;
        public int charge_remain = 5;

        protected bool _in_use = false;
		protected use_item_manager _manager = null;
		protected base_ctrl _pawn = null;

		protected virtual void Start() {
			_manager = FindObjectOfType<use_item_manager>();
		}

        public virtual bool can_use() {
            return !_in_use && charge_remain > 0;
        }

        public virtual bool use() {
            if (!can_use())
                return false;
            --charge_remain;
			refresh_visual();
            return true;
        }

        public virtual bool stop_use() {
            _in_use = false;
            return true;
        }


		public virtual void add_charge() {
			++charge_remain;
			if (charge_remain > max_capacity)
				charge_remain = max_capacity;
			refresh_visual();
		}

		public virtual void refresh_visual() {
			if (_manager != null)
				_manager.refresh_visual();
		}

        public virtual int type_id() {
            return 0;
        }

        // IComparable
        public int CompareTo(als_item x) {
            return type_id().CompareTo(x.type_id());
        }


		public virtual void set_active_pawn(base_ctrl pawn) {
			_pawn = pawn;
		}

    }
}
