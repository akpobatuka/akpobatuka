using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AssemblyCSharp {
	public class air_bomb : MonoBehaviour {
		public float count_down_time = 5f;
		public float action_time = .5f;
		public float force = 10f;
		
		Rigidbody2D _rbody = null;
		Effector2D _effect = null;
		
		
		float _count_down = -1f;
		float _boom_time = -1f;
		
		
		// initialization
		void Start () {
			_rbody = GetComponent<Rigidbody2D>();
			_effect = GetComponent<Effector2D>();


		}
		
		void FixedUpdate() {
			if (_count_down > 0f) { // обратный отсчет
				_count_down -= Time.fixedDeltaTime;
				
				if (_count_down <= 0f) {
					_effect.enabled = true;
					_boom_time = action_time;
					// ????? boom animation
				}
			}
			else if (_boom_time > 0f) { // взрыв
				_boom_time -= Time.fixedDeltaTime;
				
				if (_boom_time <= 0f) {
					_effect.enabled = false;
					hide();
				}
			}
			
			
		}

		
		/// <summary>
        /// спрятать
        /// </summary>
		public void hide() {
			gameObject.SetActive(false);
			
		}
		
		/// <summary>
        /// ожонь
        /// </summary>
		public void fire(Vector2 position, Vector2 direction) {
			gameObject.SetActive(true);
			_rbody.position = position;
			_rbody.AddForce(direction * force);
			_count_down = count_down_time;
			
		}

		public bool is_active {
			get { return gameObject.activeSelf; }
		}
		
		
	}

}