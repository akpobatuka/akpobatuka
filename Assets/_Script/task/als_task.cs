using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_task : MonoBehaviour {
        [Header("task")]
        public int id;

        public virtual bool is_completed() {
            return false;
        }
	}
}
