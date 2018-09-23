using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_task_time_trial : als_task {
        protected int _time2beat = 0;

        void Start() {
            var lvl = als_save.g().current_level();
            if (lvl != null)
                _time2beat = lvl.blue_token_time(id);
        }


        public override bool is_completed() {
            return (int)Time.timeSinceLevelLoad <= time2beat;
        }


        public int time2beat {
            get { return _time2beat; }
        }
	}
}
