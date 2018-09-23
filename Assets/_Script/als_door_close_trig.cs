using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_door_close_trig : MonoBehaviour {
			als_door_control _door_control;

			// Use this for initialization
			void Start () {
					_door_control = GetComponentInParent<als_door_control> ();
			}
					

			void OnTriggerEnter2D(Collider2D other) {
					_door_control.close_door ();
			}
	}
}