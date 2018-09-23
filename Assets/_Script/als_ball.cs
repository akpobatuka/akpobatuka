using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	[RequireComponent(typeof(Rigidbody2D))]
	public class als_ball : MonoBehaviour {
		public Transform[] goal_points; // корзины
        public Vector2 min_vel; // мин. скорость, при которой применяется навешивание
        public Vector2 max_vel; // макс. скорость, при которой применяется навешивание
		public float throw_height_min; // min высота броска
        public float throw_height_max; // max высота броска
		public float dist2compute = 10f; // максимальное расстояние до цели при котором используется рассчет

		Rigidbody2D _rbody = null;
		int _grab_count = 0;
		als_basketball_score _score = null;
		Collider2D _coll = null;


		void Awake() {
			_rbody = GetComponent<Rigidbody2D>();
			_coll = GetComponent<Collider2D>();
			_score = Object.FindObjectOfType<als_basketball_score>();
		}

		public void ball_grabbed() {
			++_grab_count;
			if (_grab_count > 2)
				_grab_count = 2;
			Debug.Log("GRA _grab_count = " + _grab_count);
			_score.goal_point = 2;
		}

        // мяч выпущен
        public void ball_released() {
			--_grab_count;
			Debug.Log("REL _grab_count = " + _grab_count);
			if (_grab_count > 0)
				return;
			_score.goal_point = (_score.point3_zone.IsTouching(_coll) ? 3 : 2);


            // запуск мяча навесом, по параболе
            // p1 - точка попадания в корзину
            // p0 - текущая точка, в которой находится мяч
            // V - скорость мяча, в момент броска, необходимая для попадания в точку p1
            // a - ускорение свободного падения (имеет только y координату)
            // t - время, через которое мяч прилетит в точку p1
            // p1 = p0 + V * t + a * (t ^ 2) / 2 -- общий закон
            // t = [ -Vy +- sqrt(Vy^2 + 2 * a * (p1 - p0)) ] / a -- время, определенное по y координатам (если мяч летит навесом, то одно и то же начение y координаты он пройдет 2 раза)
            // Vy^2 + 2 * a * (p1 - p0) = 0 -- уравнение положения мяча в верхней точке броска, отсюда у скорость необходимая для прохождения этой точки V=2*a*(p0-p1), где p1 - это throw_height
            // Vx = (p1 - p0) / t -- скорость по x координате (часть с ускорением отсутствует, т.к. a.x == 0)

            Vector2 V = _rbody.velocity; // начальная скорость мяча
            Vector2 p0 = _rbody.position; // начальное положения мяча

            // определение корзины
            Vector2 p = Vector2.zero; // положение корзины
            Vector2 delta = Vector2.zero; // разница (корзина - мяч)
            foreach (var gp in goal_points) {
                p = gp.position;
                delta = p - p0;
                if (delta.x * V.x > 0f)
                    break;
            }
            Debug.Log("ball_released 1");
            if (delta.sqrMagnitude > dist2compute * dist2compute)
                return;
            // END определение корзины


            // определение скорости по высоте броска
            float t; // время
            /*if (-delta.y > throw_height_min) { // точка броска выше корзины (с запасом)
                //t = 1f; // будем кидать вниз
                if (V.y > 0f) V.y = 0f;
				Debug.Log("ball_released 2.1");
            }
            else {*/
                //t = -1f; // будем кидать вверх навесом
				float throw_height = delta.y + Mathf.Lerp(throw_height_min, throw_height_max, (V.normalized.y + 1f) * .5f);
				V.y = -2f * Physics2D.gravity.y * throw_height;
                if (V.y < 0f) return; // гравитация направлен вверх
                V.y = Mathf.Sqrt(V.y);
				Debug.Log("ball_released 2.2");
            //}

            /*if (V.y > max_vel.y) // слишком большая скорость
                return;
            */
            

            float D = V.y * V.y + 2f * Physics2D.gravity.y * delta.y;
			Debug.Log("ball_released 4");
            if (D < utils.small_float) // слишком маленькая скорость
                return; // *
			Debug.Log("ball_released 5");
            t = (-V.y - Mathf.Sqrt(D)) / Physics2D.gravity.y; // -1 => нужно получить позднее время, при падении мяча вниз
            if (t < utils.small_float) // время попадания не определено
                return; // *

            V.x = delta.x / t;

			/*if (!utils.is_between(Mathf.Abs(V.x), min_vel.x, max_vel.x)) // не проходит ограничение горизонтальной скорости
                return; // *
*/
			Debug.Log("ball_released 6");
			_rbody.velocity = V; // навешиваем
        }
		
		public bool is_grabbed {
			get { 
				return _grab_count > 0;
			}
		}
	}
}