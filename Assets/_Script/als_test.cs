using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class als_test : MonoBehaviour {
	public GameObject indicator;
	public float visibleZ;
	public float invisibleZ;

	void OnBecameVisible() {
		var p = indicator.transform.position;
		p.z = visibleZ;
		indicator.transform.position = p;
	}

	void OnBecameInvisible() {
		var p = indicator.transform.position;
		p.z = invisibleZ;
		indicator.transform.position = p;
	}

}
