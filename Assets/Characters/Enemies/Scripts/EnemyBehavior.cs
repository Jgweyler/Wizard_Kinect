using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private GameObject player; //Referencia del jugador.
    private bool player_spotted = false; //Determina si este enemigo está viendo al jugador.

    // Use this for initialization
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (player_spotted) {
            LookOutPlayer();
        }
	}

    void OnTriggerEnter(Collider other) {
        //Debug.Log("Collision detected with trigger object " + other.name);
        if (other.tag == "Player") {
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

    void LookOutPlayer(){
        transform.LookAt(player.transform);
    }
}
