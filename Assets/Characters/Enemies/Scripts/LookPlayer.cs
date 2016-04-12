using UnityEngine;
using System.Collections;

public class LookPlayer : MonoBehaviour {

    private GameObject player; //Referencia del jugador.
    private bool player_spotted = false; //Determina si este enemigo está viendo al jugador.
    private float speed = 12f;
    private GameObject self; //Referencia al enemigo en sí. (Padre). Un enemigo está compuesto por dos Objectos vacíos con un collider cada uno. Esto es así
                             //Porque Unity no deja asignar dos eventos onTrigger diferentes a un mismo objeto.
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


}
