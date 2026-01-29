using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public enum TrolleyDirection
{
    Right,
    Left,
    Random,
}

public class TrolleyWaypointController : MonoBehaviour
{
    public SplineAnimate anim;

    [Header("Containers (each has ONLY one spline)")]
    public SplineContainer Trunk;
    public SplineContainer Left;
    public SplineContainer Right;

    private string junctionTag = "SplineTrigger"; // triggers the train to turn left or right
    private string mergeTag = "MergeTrigger";     // triggers the train to drive back to common path or trunk

    [HideInInspector] public TrolleyDirection nextDir;

    private enum TrackState
    {
        OnTrunk,
        OnLeft,
        OnRight,
    }
    private TrackState state = TrackState.OnTrunk;

    void Start()
    {
        nextDir = GameState.nextDir;
        if (!anim) anim = GetComponent<SplineAnimate>();
        GoToTrunk(); // start with common path or trunk
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // triggers turn left or right here
        if (other.CompareTag(junctionTag) && state == TrackState.OnTrunk)
        {
            if (nextDir == TrolleyDirection.Random)
            {
                // Random.Range includes lower but excludes upper when used with ints
                nextDir = (TrolleyDirection)Random.Range(0, 2);
            }

            switch (nextDir)
            {
                case TrolleyDirection.Right:
                    GoToRight();
                    break;
                case TrolleyDirection.Left:
                    GoToLeft();
                    break;
                default:
                    // Should not get here
                    Debug.Log("Invalid trolley direction selected");
                    break;
            }
        }

        // 2) triggers anim on common path or trunk
        if (other.CompareTag(mergeTag) && (state == TrackState.OnLeft || state == TrackState.OnRight))
        {
            GoToTrunk();
            nextDir = GameState.nextDir; // Reset branch selection
            return;
        }
    }

    void GoToTrunk()
    {
        state = TrackState.OnTrunk;
        anim.Container = Trunk;
        SafeRestart();
    }

    void GoToLeft()
    {
        state = TrackState.OnLeft;
        anim.Container = Left;
        SafeRestart();
    }

    void GoToRight()
    {
        state = TrackState.OnRight;
        anim.Container = Right;
        SafeRestart();
    }

    void SafeRestart()
    {
        //
        try
        {
            anim.Restart(true);
            return;
        }
        catch { }

        //
        anim.enabled = false;
        anim.enabled = true;
    }

    public void OnClickLeft()
    {
        if (state == TrackState.OnTrunk)
        {
            nextDir = TrolleyDirection.Left;
            Debug.Log("Track switched. Left track selected");
        }

    }

    public void OnClickRight()
    {
        if (state == TrackState.OnTrunk)
        {
            nextDir = TrolleyDirection.Right;
            Debug.Log("Track switched. Right track selected");
        }
    }

    /*[Header("Physics Settings")]
    // Impact force applied to the victim (higher value = flies further)
    public float impactForce = 15f;

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
    }*/
}