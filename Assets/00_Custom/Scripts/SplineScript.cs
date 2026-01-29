using UnityEngine;
using UnityEngine.Splines;

public class SplineScriptTest : MonoBehaviour
{
    public SplineAnimate anim;

    [Header("Containers (each has ONLY one spline)")]
    public SplineContainer Trunk;
    public SplineContainer Left;
    public SplineContainer Right;

    private string junctionTag = "SplineTrigger"; // triggers the train to turn left or right
    private string mergeTag = "MergeTrigger";     // triggers the train to drive back to common path or trunk

    [HideInInspector] public TrolleyDirection nextDir;

    private enum TrackState { OnTrunk, OnLeft, OnRight }
    private TrackState state = TrackState.OnTrunk;

    void Start()
    {
        nextDir = GameState.nextDir;
        if (!anim) anim = GetComponent<SplineAnimate>();
        GoToTrunk(); // start with common path or trunk
    }

    void Update()
    {
        // press 1 to turn left, press 2 to turn right; have to press before trigger box
        //if (Input.GetKeyDown(KeyCode.Alpha1)) goLeft = true;
        //if (Input.GetKeyDown(KeyCode.Alpha2)) goLeft = false;
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
            Debug.Log("Track switched. Forward track selected");
        }
    }
}
