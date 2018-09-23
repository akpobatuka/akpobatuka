using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Xml;

namespace AssemblyCSharp {
	static class utils {
		public const float small_float = .0001f;
		public const int layer_GrabsObtacle = 20;

        // пустое действие
		public static void empty_action() {}

        // ИД текущего уровеня
        public static int current_level_id() {
            int level_id = -1;
            string s = SceneManager.GetActiveScene().name;
            if (s.StartsWith("level") && s.Length == 7)
                int.TryParse(s.Substring(5, 2), out level_id);
			else if (s.StartsWith("gb_level") && s.Length == 10)
				int.TryParse(s.Substring(8, 2), out level_id);
            return level_id;
        }
		
		
		// перезагрука уровня
		public static void restart_level() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		
		
		

		public static GameObject resource_instantiate(string prefab_path, Transform parent = null, bool identity_xform = false) {
			var prefab = Resources.Load<GameObject>(prefab_path);
			var obj = Object.Instantiate(prefab, parent) as GameObject;

			if (identity_xform) {
                var t = obj.transform;
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;

				var rect = obj.GetComponent<RectTransform>();
				if (rect != null) {
					rect.anchoredPosition = Vector2.zero;
					rect.sizeDelta = Vector2.one;
				}
			}

			return obj;
		}



        // для сортировки объектов по имени
		public static int compare_object_by_name(GameObject x, GameObject y) {
			if (x == y) return 0;
			if (x == null) return -1;
			return y == null ? 1 : string.Compare(x.name, y.name);
		}


        // мировые координаты прямоугольника
		public static Rect GetWorldRect(this RectTransform xform)
		{
			var corners = new Vector3[4];
			xform.GetWorldCorners (corners);
			return new Rect (corners [0].x, corners [0].y
			                 , corners [2].x - corners [0].x, corners [2].y - corners [0].y);
		}
        // получение угла ограниченного настройками Joint'а
		public static float get_limited (this JointLimits lm, float angle)
		{
			if (angle < lm.min)
				return lm.min;
			else if (angle > lm.max)
				return lm.max;			
			return angle;
		}
        // получение разницы и угла ограниченного настройками Joint'а
		public static float get_limited_delta (this JointLimits lm, ref float angle)
		{
			float delta = 0f;
			if (angle < lm.min) {
				delta = angle - lm.min;
				angle = lm.min;
			}
			else if (angle > lm.max) {
				delta = angle - lm.max;
				angle = lm.max;
			}
			return delta;
		}

        // углы между векторами
		public static float get_dir_to (this Vector2 from, Vector2 to) {
			return Mathf.Sign(from.y * to.x - from.x * to.y);
		}
		// угол между векторами с учетом знака
		public static float get_angle_to (this Vector2 from, Vector2 to) {
			return Mathf.Atan2(from.y * to.x - from.x * to.y, Vector2.Dot(from, to)) * Mathf.Rad2Deg;
			//return Vector2.Angle(from, to) * from.get_dir_to(to);
		}
		/*public static void get_angle_to (this Vector2 from, Vector2 to, ref float angle, ref float dir) {
			angle = Vector2.Angle(from, to);
			dir = from.get_dir_to(to);
		}*/
        // перпендикуляр (против часовой стрелки)
		public static Vector2 perp (this Vector2 vector) {
			return new Vector2(-vector.y, vector.x);
		}
        // проэкция на нормаль 
        public static Vector2 projection(this Vector2 vector, Vector2 normal) {
            return normal * Vector2.Dot(normal, vector);
        }
        // вектор отражения от поверхности с нормалью normal
        public static Vector2 reflection(this Vector2 vector, Vector2 normal) {
			return vector.projection(normal) - vector.projection(normal.perp());
        }
        


		public static Vector2 vec2 (this Vector3 vector) {
			return new Vector2(vector.x, vector.y);
		}
        // поэлементный lerp вектора
		public static Vector3 lerp_individual (Vector3 a, Vector3 b, Vector3 t) {
			return new Vector3(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z));
		}
        // вектор повернутый на указаный градус
		public static Vector2 Rotate(this Vector2 v, float degrees) {
			float radians = degrees * Mathf.Deg2Rad;
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);

			float tx = v.x;
			float ty = v.y;

			return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
		}
		
		public static bool almost_equal(float a, float b) {
			return almost_equal(a, b, small_float);
		}
		public static bool almost_equal(float a, float b, float delta) {
			return a - delta < b && a + delta > b;
		}
		public static bool almost_equal(Vector2 a, Vector2 b) {
			return almost_equal(a, b, small_float);
		}
		public static bool almost_equal(Vector2 a, Vector2 b, float delta) {
			return almost_equal(a.x, b.x, delta) && almost_equal(a.y, b.y, delta);
		}
		public static bool almost_equal(Vector2 a, Vector2 b, Vector2 delta) {
			return almost_equal(a.x, b.x, delta.x) && almost_equal(a.y, b.y, delta.y);
		}
		

		public static bool is_between(float value, float min, float max) {
			return value >= min && value <= max;
		}

		public static float between(float value, float min, float max) {
			if (value > max)
				return max;
			if (value < min)
				return min;
			return value;
		}

		public static bool bbetween(ref float value, float min, float max) {
			if (value < min) {
				value = min;
				return false;
			}
			if (value > max) {
				value = max;
				return false;
			}
			return true;
		}

		// делает угол между -180 и +180
		public static float clear_angle(float angle) {
			if (is_between(angle, -180f, 180f))
				return angle;
			angle = angle % 360f;
			if (is_between(angle, -180f, 180f))
				return angle;
			angle += angle < -180 ? 360f : -360f;
			return angle;
		}

		public static Vector2 default_gravity() {
			return new Vector2(0f, -9.81f);
		}

        // быстрая длина вектора
		public static float q_dist(this Vector2 v) {
			float dx = v.x;
			float dy = v.y;
			if ( dx < 0 ) dx = -dx;
			if ( dy < 0 ) dy = -dy;
			if ( dx < dy )  return 0.961f*dy+0.398f*dx;  
			else    return 0.961f*dx+0.398f*dy;
		}

        // битовые операции
        // установка бита в int значении
        public static void set_bit(ref int int_value, int bit_id, bool bit_value) {
            int mask = 1 << bit_id;
            if (bit_value)
                int_value |= mask;
            else {
                mask = ~mask;
                int_value &= mask;
            }
        }
        // получение бита в int значении
        public static bool get_bit(int int_value, int bit_id) {
            int mask = 1 << bit_id;
            mask &= int_value;
            return mask != 0;
        }

		// кол-во установленных битов
        public static int bit_count(int int_value) {
			int count = 0;
			while (int_value != 0) {
				if ((int_value & 0x1) == 0x1)
					++count;
				int_value >>= 1;
			}
            return count;
        }
		


        // строку в число 
        public static int str2int(string str, int default_int = 0) {
            int res = default_int;
            return int.TryParse(str, out res) ? res : default_int;
        }
		// время в строку
		public static string time_str(int time) {
			int mm = time / 60;
			int ss = time % 60;
			return mm.ToString("00") + ":" + ss.ToString("00");
		}

        // добавление смещения к главной текстуре материала
        public static void add_main_tex_offset(this Material mat, Vector2 offset) {
            var o = mat.mainTextureOffset + offset;
            if (o.x > 1f)
                o.x -= 1f;
            else if (o.x < 0f)
                o.x += 1f;

            if (o.y > 1f)
                o.y -= 1f;
            else if (o.y < 0f)
                o.y += 1f;

            mat.mainTextureOffset = o;
        }


		// xml
		public static int get_int_attribute(this XmlNode node, string name, int default_int) {
			var i = node.Attributes.GetNamedItem(name);
			if (i == null)
				return default_int;
			return utils.str2int(i.InnerText, default_int);
		}
		public static string get_str_attribute(this XmlNode node, string name, string default_str) {
			var i = node.Attributes.GetNamedItem(name);
			if (i == null || string.IsNullOrEmpty(i.InnerText))
				return default_str;
			return i.InnerText;
		}

		
		// Transform
		public static void posXY(this Transform xf, Vector2 pos) {
			var p = xf.position;
			p.x = pos.x; p.y = pos.y;
			xf.position = p;
		}
		public static void rotateZ(this Transform xf, float angle) {
			var r = xf.localEulerAngles;
			r.z = angle;
			xf.localEulerAngles = r;
		}
		
		// Rigidbody2D
		public static void clean_rotation(this Rigidbody2D rb) {
			rb.rotation = clear_angle(rb.rotation);
		}
		
		
		
		// convertion
		public static string string_from_bytes(byte[] b) {
            return System.Text.Encoding.Default.GetString(b);
        }
		public static byte[] bytes_from_string(string s) {
            return System.Text.Encoding.Default.GetBytes(s);
        }
		
		
		
		/*public static void FromMatrix4x4(this Transform transform, Matrix4x4 matrix) {
			transform.localScale = matrix.GetScale();
			transform.rotation = matrix.GetRotation();
			transform.position = matrix.GetPosition();
		}
		
		public static Quaternion GetRotation(this Matrix4x4 matrix) {
			var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
			var w = 4 * qw;
			var qx = (matrix.m21 - matrix.m12) / w;
			var qy = (matrix.m02 - matrix.m20) / w;
			var qz = (matrix.m10 - matrix.m01) / w;
			
			return new Quaternion(qx, qy, qz, qw);
		}
		
		public static Vector3 GetPosition(this Matrix4x4 matrix) {
			var x = matrix.m03;
			var y = matrix.m13;
			var z = matrix.m23;
			
			return new Vector3(x, y, z);
		}
		
		public static Vector3 GetScale(this Matrix4x4 m) {
			var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
			var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
			var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
			
			return new Vector3(x, y, z);
		}*/


	}
}