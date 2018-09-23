using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp {
	public class als_cut_video : MonoBehaviour {
		public string resources_path = "";
		public float frame_step = .3f;

		Sprite[] _sprite_frames;
		Image _image;
		int _frame = 0;
		float _frame_proc = 0f;

		// Use this for initialization
		void Start () {
			_sprite_frames = Resources.LoadAll<Sprite>(resources_path);
			_frame = 0;
			_frame_proc = 0;
			_image = GetComponent<Image>();
			set_frame();
		}
		
		// Update is called once per frame
		void Update () {
			_frame_proc += Time.deltaTime;
			if (_frame_proc > frame_step) {
				++_frame;
				if (_frame >= _sprite_frames.Length)
					_frame = 0;
				_frame_proc = 0;
				set_frame();
			}
		}

		void set_frame() {
			_image.sprite = _sprite_frames[_frame];
		}
	}
}
