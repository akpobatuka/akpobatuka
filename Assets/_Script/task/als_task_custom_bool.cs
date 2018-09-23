using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_task_custom_bool : als_task_custom {
        [Header("task custom bool")]
        public bool completed = false;

        public void succeed() {
            completed = true;
			check_completed();
        }
        public void failed() {
            completed = false;
        }
        
        public override bool is_completed() {
            return completed;
        }
	}
}
