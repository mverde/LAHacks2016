using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	LineRenderer line;

	void Start()
	{
		line = gameObject.GetComponent<LineRenderer> ();
		line.enabled = false;
		gameObject.GetComponent<Light> ().enabled = false;
	}


	void Update()
	{
        if (Input.GetMouseButton(1))
        {
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
        }
	}

	public void LaserLogistics()
	{
		StopCoroutine ("FireLaser");
		StartCoroutine ("FireLaser");	
	}

	IEnumerator FireLaser()
	{
		line.enabled = true;
		gameObject.GetComponent<Light> ().enabled = true;
		while(Input.GetMouseButton(1))
		{
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit; 
			line.SetPosition(0, ray.origin);

			if (Physics.Raycast (ray, out hit, 100)) 
				line.SetPosition (1, hit.point);
			else
				line.SetPosition(1, ray.GetPoint(100));

			yield return null;
		}
		line.enabled=false;
		gameObject.GetComponent<Light> ().enabled = false;
	}
}
