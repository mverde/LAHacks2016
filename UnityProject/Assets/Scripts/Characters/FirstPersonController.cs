using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class FirstPersonController : MonoBehaviour {
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed = 8f;
    public float jumpForce = 600f;
    public LayerMask groundedMask;

    private Transform cameraT;
    private float verticalLookRotation;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private GravityBody body;

    private bool grounded = false;
    private bool shouldJump = false;

	private void Start () {
        cameraT = Camera.main.transform;
        body = GetComponent<GravityBody>();
	}
	
	private void Update () {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60f, 60f);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        if (Input.GetButtonDown("Jump"))
        {
            body.GetBody().AddForce(transform.up * jumpForce);
            if (grounded)
            {
                shouldJump = true;
                body.GetBody().AddForce(transform.up * jumpForce);
            }
            else
            {
                shouldJump = false;
            }
        }
        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + 0.1f, groundedMask))
        {
            shouldJump = true;
            grounded = true;
        }
    }

    private void FixedUpdate()
    {
        body.GetBody().MovePosition(body.GetBody().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
