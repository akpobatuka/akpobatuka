using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_door_key : MonoBehaviour {
			public Vector3 spin_speed;
			public Transform key_mesh;
			public ParticleSystem after_collect_particles;

			als_door_control _door_control;

			// Use this for initialization
			void Start () {
					_door_control = GetComponentInParent<als_door_control> ();
			}

			void OnTriggerEnter2D(Collider2D other) {
					//StartCoroutine (turn_particles ());
					key_mesh.gameObject.SetActive (false);
					after_collect_particles.gameObject.SetActive (true);
					_door_control.open_door ();
			}

			void FixedUpdate() {
					key_mesh.Rotate(spin_speed);
			}

			IEnumerator turn_particles() {
					key_mesh.gameObject.SetActive (false);
					after_collect_particles.gameObject.SetActive (true);
					yield return new WaitForSeconds (1f);
					transform.parent.gameObject.SetActive(false);
					//after_collect_particles.gameObject.SetActive (false);
			}

	}
}