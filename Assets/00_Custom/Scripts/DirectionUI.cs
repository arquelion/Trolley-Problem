using UnityEngine;
using UnityEngine.UI;

public class DirectionUI : MonoBehaviour
{
    [SerializeField] private TrolleyWaypointController trolley;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject random;

    // Update is called once per frame
    void Update()
    {
        switch (trolley.nextDir)
        {
            case TrolleyDirection.Right:
                leftArrow.SetActive(false);
                random.SetActive(false);
                rightArrow.SetActive(true);
                break;
            case TrolleyDirection.Left:
                leftArrow.SetActive(true);
                random.SetActive(false);
                rightArrow.SetActive(false);
                break;
            case TrolleyDirection.Random:
                leftArrow.SetActive(false);
                random.SetActive(true);
                rightArrow.SetActive(false);
                break;
            default:
                // should not get here
                Debug.Log("Invalid direction to display");
                break;
        }
    }
}
