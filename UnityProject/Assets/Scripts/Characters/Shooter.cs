using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	public GameObject shot;

	public void Shoot()
	{
		Instantiate (shot, transform.position, transform.rotation);
	}
}
