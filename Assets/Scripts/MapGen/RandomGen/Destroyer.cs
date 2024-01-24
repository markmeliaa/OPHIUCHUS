using UnityEngine;

public class Destroyer : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		Destroy(other.gameObject);
	}
}
