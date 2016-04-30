using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	public int currentWeapon = 0;
	private int nrWeapons;
	public GameObject[] shot;


	public void Shoot()
	{
		//print ("Shoot");
		Instantiate (shot[currentWeapon], transform.position, transform.rotation);
		//print ("Current Weapon: " + currentWeapon + " Number of Weapons: " + nrWeapons);
	}

	void Start() {
		nrWeapons = shot.Length;
		SwitchWeapon(currentWeapon); // Set default gun
	}
		
//
//	void Update () 
//	{
//		//print ("Current Weapon Update: " + currentWeapon + " Number of Weapons Update: " + nrWeapons);
//		for (int j=1; j <= nrWeapons; j++)
//		{ 
//			if (Input.GetKeyDown("" + j))
//			{
//				currentWeapon = j-1;
//				print ("Update: Call Switch Weapon" + j);
//				SwitchWeapon(currentWeapon);
//			}
//		}
//	}

	void SwitchWeapon(int index) {
		//print ("Switch weapon");
		for (int i=0; i < nrWeapons; i++)
		{
			if (i == index)
			{
				shot[i].gameObject.SetActive(true);
			} 
			else 
			{ 
				shot[i].gameObject.SetActive(false);
			}
		}
	}
		
}
