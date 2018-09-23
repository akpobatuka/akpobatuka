using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp {
	public class als_ui_text_from_note : MonoBehaviour {
		public int note_id;

		void Start() {
			var t = GetComponent<Text>();
			if (t != null)
				t.text = als_lang_g_note.g().note(note_id);
		}
		

	}
}

