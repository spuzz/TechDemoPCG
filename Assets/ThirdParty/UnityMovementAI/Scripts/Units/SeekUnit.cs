using UnityEngine;

namespace UnityMovementAI
{
    public class SeekUnit : MonoBehaviour
    {
        public Transform target;

        SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            
        }

        void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, target.position) > 10)
            {
                Vector3 accel = steeringBasics.Seek(target.position);

                steeringBasics.Steer(accel);
                steeringBasics.LookWhereYoureGoing();
            }
        }
    }
}