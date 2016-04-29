using UnityEngine;

public class SplineOrbiter : MonoBehaviour
{
    public BezierSpline spline;
    public float duration;
    public bool lookForward;

    private float progress;
    private bool goingForward = true;

    private void Update()
    {
        if (goingForward)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                progress -= 1f;
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
