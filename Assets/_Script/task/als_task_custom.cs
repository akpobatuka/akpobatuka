using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
    public class als_task_custom : als_task {
		[Header("task custom")]
		public UnityEvent on_complete;
        protected int _name_id;

        void Start() {
            var lvl = als_save.g().current_level();
            if (lvl != null)
                _name_id = lvl.yellow_token_id(id);
        }


        public int name_id {
            get { return _name_id; }
        }

		protected virtual void check_completed() {
			if (is_completed())
				on_complete.Invoke();
		}

	}
}
