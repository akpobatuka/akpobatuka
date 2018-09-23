using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp {
	public class als_fader {
		public float dot_value { get; set; }
		public float dash_speed { get; set; }

		readonly Image _fader = null;
		MonoBehaviour _owner = null;

		public als_fader (MonoBehaviour owner) {
			var o = owner.transform.Find("Fader");
			if (o != null) {
				_fader = o.GetComponent<Image>();
				_owner = owner;
			}
		}

		public void fade_dot() {
			if (_fader == null)
				return;
			var c = _fader.color;
			c.a = dot_value;
			_fader.color = c;
			_fader.gameObject.SetActive(true);
		}
		public void unfade_dot() {
			if (_fader == null)
				return;
			_fader.gameObject.SetActive(false);
		}

		public void unfade_dash() {
			if (_fader == null)
				return;

			var c = _fader.color;
			c.a = 1f;
			_fader.color = c;
			_fader.gameObject.SetActive(true);
			_owner.StartCoroutine(do_unfade());
		}


		IEnumerator do_unfade() {
			while (true) {
				var c = _fader.color;
				if (c.a < .01f) {
					unfade_dot();
					yield break;
				}

				c.a -= dash_speed * Time.deltaTime;
				if (c.a < 0f)
					c.a = 0f;
				_fader.color = c;

				yield return null;
			}
		}



	}
}

