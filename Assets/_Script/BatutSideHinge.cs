using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class BatutSideHinge : MonoBehaviour {
		public bool negative_speed = false;
		public float m_max_speed; // abs значение максимальной скорости
		public float m_max_torque; // усиленный режим
		public float m_start_speed; // abs значение стартовой скорости
		public float m_start_torque; // обычный режим силы
		
		public float m_speed_magnifier;
		public float m_torque_magnifier;
		
		HingeJoint2D _joint;
        protected float _lazy_update_time = 0f;

		const float lazy_update_delta_time = .05f;
		JointAngleLimits2D _default_limits;
		JointAngleLimits2D _stop_limits = new JointAngleLimits2D();

		bool _return = false;

        //this being here will save GC allocs
		JointMotor2D _mmm = new JointMotor2D();
		float _aaa = 0f;
        //\this being here will save GC allocs


		// Use this for initialization
		void Start () {
			_joint = (HingeJoint2D)gameObject.GetComponent("HingeJoint2D");
			_joint.connectedAnchor = transform.TransformPoint(_joint.anchor);
			_default_limits = _joint.limits;
			_stop_limits.min = 0f;
			_stop_limits.max = 0f;
		}
		
		// Update is called once per frame
		void FixedUpdate () {
            _lazy_update_time -= Time.fixedDeltaTime;
            if (_lazy_update_time > 0f) return;
            _lazy_update_time = lazy_update_delta_time;

			_aaa = Mathf.Abs(_joint.jointAngle);
			if (_aaa < 1f) {
				if (_return) {
					_joint.limits = _stop_limits;
					_return = false;
				}
			}
			else if (!_return) {
				_return = true;
				_joint.limits = _default_limits;
			}

            _mmm.motorSpeed = Mathf.Exp(_aaa);
            _mmm.maxMotorTorque = _mmm.motorSpeed * m_torque_magnifier;
            _mmm.motorSpeed *= m_speed_magnifier;

            _mmm.motorSpeed += m_start_speed;
            if (_mmm.motorSpeed > m_max_speed)
                _mmm.motorSpeed = m_max_speed;

            _mmm.maxMotorTorque += m_start_torque;
            if (_mmm.maxMotorTorque > m_max_torque)
                _mmm.maxMotorTorque = m_max_torque;

			if (negative_speed)
				_mmm.motorSpeed = -_mmm.motorSpeed;

            _joint.motor = _mmm;
		}
	}
}