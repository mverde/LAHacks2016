using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DontGoThroughThings))]
public class ShipController : MonoBehaviour
{
    public float mouseSensitivityX = 0f;
    public float mouseSensitivityY = 0f;
    public float maxMoveSpeed = 0f;
    public float strafeSpeed = 0f;
    public float turnSpeed = 0f;
    public float damping = 0f;
    public float accelerationSpeed = 0f;

    private float moveSpeed;
    private Transform cameraT;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private Rigidbody body;
    private float roll, pitch, yaw, power;
    private Vector3 strafe;
    private Shooter shooter;

    private void Start()
    {
        cameraT = Camera.main.transform;
        cameraT.parent = transform;
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        shooter = GetComponent<Shooter>();
        moveSpeed = 0f;
    }

    private void Update()
    {
        roll = Input.GetAxis("Roll");
        pitch = -Input.GetAxis("Mouse Y") * mouseSensitivityY;
        yaw = Input.GetAxis("Mouse X") * mouseSensitivityX;
        strafe = new Vector3(Input.GetAxis("Horizontal") * strafeSpeed * Time.deltaTime, 0, 0);
        power = Input.GetAxis("Power") * accelerationSpeed;

        moveSpeed += power;

        if (moveSpeed > maxMoveSpeed)
        {
            moveSpeed = maxMoveSpeed;
        }
        else if (moveSpeed < 0)
        {
            moveSpeed = 0f;
        }
        print("moveSpeed: " + moveSpeed);
    }

    private void FixedUpdate()
    {
        float adjustedDamping = damping * Time.deltaTime;
        //body.AddForce(moveAmount);
        //body.AddRelativeTorque(pitch * Time.deltaTime, yaw * Time.deltaTime, roll * Time.deltaTime);
        transform.Rotate(new Vector3(pitch * turnSpeed * Time.deltaTime, yaw * turnSpeed * Time.deltaTime, roll * turnSpeed * Time.deltaTime));
        body.AddRelativeForce(0, 0, moveSpeed * Time.deltaTime);
        if (moveSpeed == 0 && body.velocity.magnitude > 0)
        {
            body.velocity *= adjustedDamping;
        }
        body.AddRelativeForce(strafe);
    }
}
