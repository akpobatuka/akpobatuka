using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp {
	public class als_light : MonoBehaviour {
		public Vector2[] orientations;


		void Start () {
			if (orientations.Length > 0) {
				int i = Random.Range(0, orientations.Length);
				if (i == orientations.Length)
					--i;
				transform.eulerAngles = orientations[i];
			}
		}
		

	}
}
