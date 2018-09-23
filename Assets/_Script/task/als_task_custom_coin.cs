using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_task_custom_coin : als_task_custom {
        [Header("task custom coin")]
        public int coin_count = 5;

        public void coin_collected() {
            --coin_count;
			check_completed();
        }
        
        public override bool is_completed() {
            return coin_count <= 0;
        }
	}
}
