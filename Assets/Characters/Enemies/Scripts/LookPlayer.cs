using UnityEngine;
using System.Collections;

public class LookPlayer : MonoBehaviour {

    private GameObject player; //Player reference.
    private bool player_spotted = false; //If the player has ben spotted by the enemy.
    private float speed = 12f;
    private GameObject self; //Parent of this gameObject. (Padre). An enemy has two empty gameObject and for each one there's a Collider.
                             //We can't assign two colliders to the same gameObject.
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        self = transform.parent.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (player_spotted)
        {
            LookOutPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision detected with trigger object " + other.name);
        if (other.tag == "Player")
        {
            player_spotted = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Collision detected with trigger object " + other.name);
        if (other.tag == "Player")
        {
            player_spotted = false;
        }
    }

    void LookOutPlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - self.transform.position);
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, targetRotation, speed * Time.deltaTime);
    }

	public bool isPlayerSpotted(){
		return player_spotted;
	}


}
