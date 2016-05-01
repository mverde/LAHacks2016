using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DontGoThroughThings))]
[RequireComponent(typeof(Shooter))]
public class ShipController2 : MonoBehaviour
{
    //speed stuff
    float speed;
    public int cruiseSpeed;
    float deltaSpeed;
    public int minSpeed;
    public int maxSpeed;
    float accel, decel;

    //turning stuff
    Vector3 angVel;
    Vector3 shipRot;
    public int sensitivity;

    public Vector3 cameraOffset; //I use (0,1,-3)

    Shooter shooter;

    void Start()
    {
        speed = cruiseSpeed;
        shooter = GetComponent<Shooter>();
    }

    void FixedUpdate()
    {

        //ANGULAR DYNAMICS//

        shipRot = transform.eulerAngles; //make sure you're getting the right child (the ship).  I don't know how they're numbered in general.

        //since angles are only stored (0,360), convert to +- 180
        if (shipRot.x > 180) shipRot.x -= 360;
        if (shipRot.y > 180) shipRot.y -= 360;
        if (shipRot.z > 180) shipRot.z -= 360;

        //vertical stick adds to the pitch velocity
        //         (*************************** this *******************************) is a nice way to get the square without losing the sign of the value
        float mouseX = Mathf.Lerp(-1, 1, Input.mousePosition.x / Screen.width);
        float mouseY = Mathf.Lerp(-1, 1, Input.mousePosition.y / Screen.height);
        angVel.x -= mouseY * Mathf.Abs(mouseY) * sensitivity * Time.fixedDeltaTime;

        //horizontal stick adds to the roll and yaw velocity... also thanks to the .5 you can't turn as fast/far sideways as you can pull up/down
        float turn = mouseX * Mathf.Abs(mouseX) * sensitivity * Time.fixedDeltaTime;
        angVel.y += turn * .5f;
        angVel.z -= turn * .5f;

        //shoulder buttons add to the roll and yaw.  No deltatime here for a quick response
        //comment out the .y parts if you don't want to turn when you hit them
        float roll = Input.GetAxis("Roll");
        //angVel.y -= 20 * roll;
        angVel.z += 50 * roll;
        speed -= 5 * roll * Time.fixedDeltaTime;


        //your angular velocity is higher when going slower, and vice versa.  There probably exists a better function for this.
        angVel /= 1 + speed * .005f;

        //this is what limits your angular velocity.  Basically hard limits it at some value due to the square magnitude, you can
        //tweak where that value is based on the coefficient
        angVel -= angVel.normalized * angVel.sqrMagnitude * .08f * Time.fixedDeltaTime;


        //and finally rotate.  
        transform.Rotate(angVel * Time.fixedDeltaTime);


        //LINEAR DYNAMICS//

        deltaSpeed = speed - cruiseSpeed;

        //This, I think, is a nice way of limiting your speed.  Your acceleration goes to zero as you approach the min/max speeds, and you initially
        //brake and accelerate a lot faster.  Could potentially do the same thing with the angular stuff.
        decel = speed - minSpeed;
        accel = maxSpeed - speed;

        //simple accelerations
        if (Input.GetAxis("Power") > 0)
            speed += accel * Time.fixedDeltaTime;
        else if (Input.GetAxis("Power") < 0)
            speed -= decel * Time.fixedDeltaTime;

        //if not accelerating or decelerating, tend toward cruise, using a similar principle to the accelerations above
        //(added clamping since it's more of a gradual slowdown/speedup)
        else if (Mathf.Abs(deltaSpeed) > .1f)
            speed -= Mathf.Clamp(deltaSpeed * Mathf.Abs(deltaSpeed), -30, 100) * Time.fixedDeltaTime;

        if (speed < 0)
            speed = 0;

        //moves camera (make sure you're GetChild()ing the camera's index)
        //I don't mind directly connecting this to the speed of the ship, because that always changes smoothly
        Camera.main.transform.localPosition = cameraOffset + new Vector3(0, 0, -deltaSpeed * .02f);

        //(**************** this ***************) is what actually makes the whole ship move through the world!
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime, Space.Self);
    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(1))
        {
            shooter.Shoot();
        }*/
    }
}
