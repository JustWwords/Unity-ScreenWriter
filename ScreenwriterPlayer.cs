using UnityEngine;

public class ScreenwriterPlayer : MonoBehaviour
{
    [SerializeField] private Screenwriter Screenwriter;
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("TRIG"))
        {
            Screenwriter.CheckActions(other.gameObject);
        }
    }
}
