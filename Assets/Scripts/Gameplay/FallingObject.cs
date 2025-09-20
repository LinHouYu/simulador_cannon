using UnityEngine;

public class FallingObject : MonoBehaviour
{
    private bool hasScored = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasScored)
        {
            hasScored = true;

            if (CompareTag("Target"))
            {
                ScoreAndTimerManager.Instance.AddScore(30);
                Destroy(gameObject, 5f);
            }
            else if (CompareTag("Wall"))
            {
                ScoreAndTimerManager.Instance.AddScore(10);
                Destroy(gameObject, 0.5f);
            }
        }
    }
}
