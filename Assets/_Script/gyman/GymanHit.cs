using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class GymanHit : MonoBehaviour {
		public bool enable_relax = false;
		public float strength = 100f;
		
		als_gymnast_ctrl m_control;
		
		
		void Start () {
			m_control = GetComponentInParent<als_gymnast_ctrl>();
		}
		
		void OnCollisionEnter2D(Collision2D coll) {
			check_in_air(coll);

			if (coll.gameObject.layer != 9) // check ground layer
				return;

			if (enable_relax && coll.relativeVelocity.sqrMagnitude > strength)
				m_control.body_relax();

		}
		void OnCollisionStay2D(Collision2D coll) {
			check_in_air(coll);
		}


		void check_in_air(Collision2D coll) {
			if (m_control.in_air && coll.gameObject.layer != utils.layer_GrabsObtacle)
				m_control.in_air_time = 0f;
		}

	}
}