using UnityEngine;

namespace UnityMovementAI
{
    public class FlockingUnit : MonoBehaviour
    {
        public float cohesionWeight = 1.5f;
        public float separationWeight = 2f;
        public float velocityMatchWeight = 1f;
        public Transform target;

        SteeringBasics steeringBasics;
        Wander2 wander;
        Cohesion cohesion;
        Separation separation;
        VelocityMatch velocityMatch;

        NearSensor sensor;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
            cohesion = GetComponent<Cohesion>();
            separation = GetComponent<Separation>();
            velocityMatch = GetComponent<VelocityMatch>();
            target = FindObjectOfType<Spawner>().transform;
            sensor = transform.Find("Sensor").GetComponent<NearSensor>();
        }

        void FixedUpdate()
        {
            Vector3 accel = Vector3.zero;

            accel += cohesion.GetSteering(sensor.targets) * cohesionWeight;
            accel += separation.GetSteering(sensor.targets) * separationWeight;
            accel += velocityMatch.GetSteering(sensor.targets) * velocityMatchWeight;

            foreach(var a in InfiniteTerrain.peaks)
            {
                
                if (Vector3.Distance(target.transform.position,a) < 100)
                {
                    accel += steeringBasics.Seek(a) * 8.0f;
                }
            }

            accel += steeringBasics.Seek(target.position) * 3.0f;
            if (accel.magnitude < 0.005f)
            {
                accel = wander.GetSteering();
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
            var pos = transform.position;
            if(pos.y < 1)
            {
                transform.position.Set(pos.x, 1, pos.z);
            }
        }
    }
}