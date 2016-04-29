using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour {
    private GravityAttractor attractor;
    private Rigidbody body;

    private void Awake()
    {
        attractor = GameObject.FindGameObjectWithTag("Attractor").GetComponent<GravityAttractor>();
        body = GetComponent<Rigidbody>();

        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        attractor.Attract(transform);
    }

    public Rigidbody GetBody()
    {
        return body;
    }
}
