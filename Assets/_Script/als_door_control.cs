using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_door_control : MonoBehaviour {
		public Transform closetrig_xf;


		Collider2D door_collider;
		Animator door_anim;

		// Use this for initialization
		void Start () {
				door_collider = GetComponent<Collider2D>();
				door_anim = GetComponent<Animator>();
		}

		public void open_door () {
			if (!door_collider.enabled)
				return;
			door_anim.SetTrigger("open");
			door_collider.enabled = false;
		}
		public void close_door() {
			if (door_collider.enabled)
				return;
			door_anim.SetTrigger ("close");
			door_collider.enabled = true;
			closetrig_xf.gameObject.SetActive (false);
		}
	}
}