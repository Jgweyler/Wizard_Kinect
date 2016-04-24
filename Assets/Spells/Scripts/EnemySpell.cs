using UnityEngine;
using System.Collections;

public class EnemySpell : MonoBehaviour {

    public float maxTimeLife = 2f;
    private GameObject player;
    private PlayerHealth playerHealth;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider other)
	{
        // If the entering collider is the player...
		if (other == player.GetComponentInChildren<BoxCollider>())
        {
            // ... the player is in range.
            playerHealth.TakeDamage(25);
			Debug.Log ("Jugador alcanzado");
            Destroy(gameObject);
        }
    }
}
