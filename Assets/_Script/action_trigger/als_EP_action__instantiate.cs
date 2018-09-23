using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_EP_action__instantiate : als_EP_action {
        [Header("instantiate")]
		public GameObject prefab;

		public override void action_start() {
            var new_obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform) as GameObject;
            new_obj.transform.localScale = Vector3.one;
		}


	}

}