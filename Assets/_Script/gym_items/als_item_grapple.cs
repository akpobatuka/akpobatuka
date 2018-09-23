using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_item_grapple : als_item {
        [Header("grapple")]
        public int raycast_layer = 1;
        public Rigidbody2D attach_body = null; // gyman arm
        public Vector2 attach_point; // on gyman arm
        public Vector2 grab_point;

        //int _mask = 0;

        DistanceJoint2D _joint = null;

        protected override void Start() {
			base.Start();
            //_mask = 1 << raycast_layer;

			if (attach_body == null)
				return;
            _joint = attach_body.gameObject.AddComponent<DistanceJoint2D>();
            _joint.enabled = false;
            _joint.autoConfigureConnectedAnchor = false;
            _joint.enableCollision = true;
            _joint.anchor = attach_point;
            _joint.connectedBody = null;
            _joint.autoConfigureDistance = false;
            _joint.maxDistanceOnly = true;
        }

        public override int type_id() {
            return 3;
        }


        public override bool use() {
            return _in_use ? do_normal() : do_grapple();
        }
        public override bool stop_use() {
            return true;
        }



        bool do_grapple() {
            if (!base.use())
                return false;

            /*var a = transform.TransformPoint(attach_point).vec2();
            RaycastHit2D rc = Physics2D.Linecast(a, grab_point, _mask);


            attach_body.AddForce((rc.point - a).normalized * 10f);
            */

			var v = attach_body.transform.TransformPoint(attach_point).vec2() - grab_point;

            _joint.distance = v.magnitude;

            _joint.connectedAnchor = grab_point;
            _joint.enabled = true;

            return true;
        }
        bool do_normal() {
            _joint.enabled = false;
            return base.stop_use();
        }



        
        

	}
}
