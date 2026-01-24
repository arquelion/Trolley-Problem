using UnityEngine;
using UnityEngine.UI;

public class DirectionUI : MonoBehaviour
{
    [SerializeField] private TrolleyWaypointController trolley;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject forwardArrow;
    [SerializeField] private GameObject random;

    // Update is called once per frame
    void Update()
    {
        switch (trolley.nextDir)
        {
            case TrolleyDirection.Forward:
                leftArrow.SetActive(false);
                random.SetActive(false);
                forwardArrow.SetActive(true);
                break;
            case TrolleyDirection.Left:
                leftArrow.SetActive(true);
                random.SetActive(false);
                forwardArrow.SetActive(false);
                break;
            case TrolleyDirection.Random:
                leftArrow.SetActive(false);
                random.SetActive(true);
                forwardArrow.SetActive(false);
                break;
            default:
                // should not get here
                Debug.Log("Invalid direction to display");
                break;
        }
    }
}
