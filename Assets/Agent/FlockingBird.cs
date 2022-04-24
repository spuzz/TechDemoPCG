using UnityEngine;
using NPBehave;

// Code base using https://github.com/sturdyspoon/unity-movement-ai
// Adapter to combine floaking with seek navigation with multiple beheaviours using NPBehave
namespace UnityMovementAI
{
    public class FlockingBird : MonoBehaviour
    {
        public float cohesionWeight = 1.5f;
        public float separationWeight = 2f;
        public float velocityMatchWeight = 1f;
        GameObject target;
        Spawner spawner;
        private Root tree;                  // The Birds behaviour tree
        private Blackboard blackboard;      // The Birds behaviour blackboard
        public int m_Behaviour = 0;         // Used to select an AI behaviour for the bird

        SteeringBasics steeringBasics;
        Wander2 wander;
        Cohesion cohesion;
        Separation separation;
        VelocityMatch velocityMatch;
        Flee flee;

        NearSensor sensor;
        Vector3 accel;
        float targetPriority = 3.0f;

        public GameObject dangerousBird;
        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
            cohesion = GetComponent<Cohesion>();
            separation = GetComponent<Separation>();
            velocityMatch = GetComponent<VelocityMatch>();
            sensor = transform.Find("Sensor").GetComponent<NearSensor>();
            spawner = FindObjectOfType<Spawner>();
            flee = GetComponent<Flee>();

            // Behaviour Tree
            tree = CreateBehaviourTree();
            blackboard = tree.Blackboard;

            // Adjust colour of agent depending on behaviour
            if(m_Behaviour == 0 || m_Behaviour == 1)
            {
                GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            else if(m_Behaviour == 2)
            {
                GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            else
            {
                GetComponent<MeshRenderer>().material.color = Color.red;
            }
            tree.Start();
        }
        private Root CreateBehaviourTree()
        {
            // Behaviours : Follow Player, Fly to Peaks, Seek Random other bird
            switch (m_Behaviour)
            {
                case 1:
                    return FollowBehaviour();
                case 2:
                    return FindPeakBehaviour();
                case 3:
                    return SeekRandomBehaviour();

                default:
                    return FollowBehaviour();
            }
        }
        private Root FollowBehaviour()
        {
            return new Root(new Sequence(
                        new Action(() => Follow())
                    ));
        }

        private Root FindPeakBehaviour()
        {
            return new Root(new Sequence(
                        new Action(() => FindPeak())
                    ));
        }
        private Root SeekRandomBehaviour()
        {
            return new Root(new Sequence(
                        new Action(() => PickTarget()),
                        new Wait(4)
                    ));
        }

        private void Follow()
        {
            // Follow the player with set priority (multiplies that vector)
            targetPriority = 3.0f;
            target = spawner.gameObject;
        }

        // Pick a random other bird to seek
        private void PickTarget()
        {
            // If no objects chase own tail until one is available!
            if(spawner.objs.Count > 0)
            {
                var random = new System.Random();

                // Irrelevent in this case
                targetPriority = 100.0f;

                // Pick random index from spawned bird list
                int index = random.Next(spawner.objs.Count - 1);
                target = spawner.objs[index].gameObject;
            }
            else
            {
                target = gameObject;
            }

            
        }

        // During map generation the tallest peak of each map chunk is found
        // If one of these is nearby to player - go to it
        private void FindPeak()
        {
            // Search all tallest peaks
            foreach (var a in InfiniteTerrain.peaks)
            {
                // Only go if close to player
                if (Vector3.Distance(spawner.transform.position, a.transform.position) < 100)
                {
                    targetPriority = 8.0f;
                    target = a;
                    return;
                }
            }
            targetPriority = 3.0f;
            target = spawner.gameObject;
        }

        void FixedUpdate()
        {
            Vector3 accel = Vector3.zero;

            // Calculate directional vector to set target
            accel += steeringBasics.Seek(target.transform.position) * targetPriority;

            // Flocking and Flee behaviour - Disable for "Dangerous bird (seeker)"
            if(m_Behaviour != 3)
            {
                accel += cohesion.GetSteering(sensor.targets) * cohesionWeight;
                accel += separation.GetSteering(sensor.targets) * separationWeight;
                accel += velocityMatch.GetSteering(sensor.targets) * velocityMatchWeight;


                if (flee)
                {
                    accel += flee.GetSteering(dangerousBird.transform.position) * 5;
                }

                if (accel.magnitude < 0.005f)
                {
                    accel = wander.GetSteering();
                }
            }

            // Apply final direction
            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
            var pos = transform.position;

            // Make sure agent doesn't reach sea level
            if(pos.y < 1)
            {
                transform.position.Set(pos.x, 1, pos.z);
            }
        }
    }
}