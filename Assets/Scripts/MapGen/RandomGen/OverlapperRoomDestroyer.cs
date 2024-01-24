using UnityEngine;

public class OverlapperRoomDestroyer : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		Destroy(other.gameObject);
	}
}
