using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class grabot_tracks_collider : MonoBehaviour	{
		Vector2 _contact_point;
		public Vector2 contact_point { get { return _contact_point; } }
		Vector2 _contact_normal;
		public Vector2 contact_normal { get { return _contact_normal; } }

		float _contact_time;
		public float contact_time { get { return _contact_time; } }


		void OnCollisionStay2D(Collision2D coll) {
			_contact_point = Vector2.zero;
			_contact_normal = Vector2.zero;
			var n = coll.contacts.Length;
			if (n < 1) return;

			foreach (var c in coll.contacts) {
				_contact_point += c.point;
				_contact_normal += c.normal;
			}

			float k = 1f / n;
			_contact_point *= k;
			_contact_normal *= k;
			_contact_time = Time.time;
		}



	}
}
