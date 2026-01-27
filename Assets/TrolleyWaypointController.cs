using UnityEngine;
using System.Collections.Generic;

public enum TrolleyDirection
{
    Forward,
    Left,
    Random,
}

public class TrolleyWaypointController : MonoBehaviour
{
    [HideInInspector] public TrolleyDirection nextDir;

    [Header("Path Settings")]
    // Common path: from start to the junction (include the junction point)
    public Transform[] commonPath;

    // Straight branch: points after the junction for the straight track
    public Transform[] forwardPath;

    // Side branch: points after the junction for the side track
    public Transform[] leftPath;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 5f;

    [Header("Physics Settings")]
    // Impact force applied to the victim (higher value = flies further)
    public float impactForce = 15f;

    // Private state variables (declared only, initialized in Start)
    private Transform[] selectedPath;
    private Queue<Transform> currentPathQueue;
    private Transform currentTargetPoint;
    private bool isOnCommonPath;

    void Start()
    {
        // === Safe Initialization ===
        // Initialize variables here to ensure they run on the Main Thread

        nextDir = GameState.nextDir;//newly tested

        isOnCommonPath = true;
        currentPathQueue = new Queue<Transform>();
       // nextDir = TrolleyDirection.Forward; // TODO: Change to forward or random depending on which scenario is chosen in the options

        // Load the common path into the queue at the start
        if (commonPath != null && commonPath.Length > 0)
        {
            foreach (Transform point in commonPath)
            {
                currentPathQueue.Enqueue(point);
            }

            // Get the first target point
            GetNextPoint();
        }
        else
        {
            Debug.LogError("Error: Common Path is not set in the Inspector!");
        }
    }

    void Update()
    {
        // If there is no target, stop execution
        if (currentTargetPoint == null) return;

        // Movement Logic: Move towards the current target point
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPoint.position, speed * Time.deltaTime);

        // Rotation Logic: Smoothly rotate towards the target direction
        Vector3 direction = currentTargetPoint.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if the trolley has reached the current target point
        if (Vector3.Distance(transform.position, currentTargetPoint.position) < 0.1f)
        {
            CheckAndLoadNextPathSegment();
            GetNextPoint();
        }
    }

    // Checks if the common path is finished and loads the selected branch
    void CheckAndLoadNextPathSegment()
    {
        // If queue is empty and we are still marked as being on the common path
        if (currentPathQueue.Count == 0 && isOnCommonPath)
        {
            isOnCommonPath = false; // We are now leaving the common path

            if (nextDir == TrolleyDirection.Random)
            {
                // Random.Range includes lower but excludes upper when used with ints
                nextDir = (TrolleyDirection)Random.Range(0, 2);
            }

            // Select the next path
            switch (nextDir)
            {
                case TrolleyDirection.Forward:
                    selectedPath = forwardPath;
                    break;
                case TrolleyDirection.Left:
                    selectedPath = leftPath;
                    break;
                default:
                    // Should not get here
                    Debug.Log("Invalid trolley direction selected");
                    break;
            }

            if (selectedPath != null)
            {
                foreach (Transform point in selectedPath)
                {
                    currentPathQueue.Enqueue(point);
                }
                Debug.Log("Entering branch: " + ((selectedPath == leftPath) ? "Side Track" : "Straight Track"));
            }
        }
    }

    // Dequeues the next point from the path list
    void GetNextPoint()
    {
        if (currentPathQueue.Count > 0)
        {
            currentTargetPoint = currentPathQueue.Dequeue();
        }
        else
        {
            currentTargetPoint = null;
            Debug.Log("Destination reached.");
        }
    }

    // === Physics Collision Logic ===
    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the tag "People"
        if (other.CompareTag("People"))
        {
            // Get the Rigidbody component from the victim
            Rigidbody personRb = other.GetComponent<Rigidbody>();

            if (personRb != null)
            {
                Debug.Log("Impact with victim: " + other.gameObject.name);

                // Calculate impact direction: forward + slightly upward
                Vector3 flyDirection = (transform.forward + Vector3.up).normalized;

                // Apply impulse force to send them flying
                personRb.AddForce(flyDirection * impactForce, ForceMode.Impulse);

                // Apply random torque (rotation) for a chaotic effect
                personRb.AddTorque(Random.insideUnitSphere * impactForce * 2f, ForceMode.Impulse);

                // Remove the tag so we don't trigger the collision logic multiple times
                other.tag = "Untagged";
            }
        }
    }

    public void OnClickLeft()
    {
        if (isOnCommonPath)
        {
            nextDir = TrolleyDirection.Left;
            Debug.Log("Track switched. Left track selected");
        }
            
    }

    public void OnClickForward()
    {
        if (isOnCommonPath)
        {
            nextDir = TrolleyDirection.Forward;
            Debug.Log("Track switched. Forward track selected");
        }
    }
}