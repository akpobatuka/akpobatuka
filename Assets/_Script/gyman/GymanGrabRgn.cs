using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class GymanGrabRgn : MonoBehaviour {
		als_grab _parent;

		// Use this for initialization
		void Start () {
			_parent = GetComponentInParent<als_grab>();
		}
		
		void OnTriggerEnter2D(Collider2D other) {
			_parent.EnterGrabRgn(other);
		}
	}
}