using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DefaultWeapon : MonoBehaviour
{
	public float moveSpeed = 1000f;
    private Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
        GetComponent<Rigidbody>().useGravity = false;
    }

	void OnCollisionEnter(){
		Destroy(gameObject); // destroy the grenade
	}

	void FixedUpdate()
	{
		transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime );
        if ((transform.position - startingPos).magnitude > 6000)
        {
            Destroy(gameObject);
        }
	}

}