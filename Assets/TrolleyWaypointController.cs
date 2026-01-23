using UnityEngine;
using System.Collections.Generic;

public class TrolleyWaypointController : MonoBehaviour
{
    [Header("Path Settings")]
    // Common path: from start to the junction (include the junction point)
    public Transform[] commonPath;

    // Straight branch: points after the junction for the straight track
    public Transform[] straightPath;

    // Side branch: points after the junction for the side track
    public Transform[] sidePath;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 5f;

    [Header("Physics Settings")]
    // Impact force applied to the victim (higher value = flies further)
    public float impactForce = 15f;

    // Private state variables (declared only, initialized in Start)
    private bool isSideTrackSelected;
    private Queue<Transform> currentPathQueue;
    private Transform currentTargetPoint;
    private bool isOnCommonPath;

    void Start()
    {
        // === Safe Initialization ===
        // Initialize variables here to ensure they run on the Main Thread
        isSideTrackSelected = false; // Default to straight track
        isOnCommonPath = true;
        currentPathQueue = new Queue<Transform>();

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

        // Input detection: Space bar to switch tracks
        // Only allow switching while on the common path (before the junction)
        if (isOnCommonPath && Input.GetKeyDown(KeyCode.Space))
        {
            isSideTrackSelected = !isSideTrackSelected;
            Debug.Log("Track switched. Side track selected: " + isSideTrackSelected);
        }

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

            // Select the path based on the boolean state
            Transform[] selectedPath = isSideTrackSelected ? sidePath : straightPath;

            if (selectedPath != null)
            {
                foreach (Transform point in selectedPath)
                {
                    currentPathQueue.Enqueue(point);
                }
                Debug.Log("Entering branch: " + (isSideTrackSelected ? "Side Track" : "Straight Track"));
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
}