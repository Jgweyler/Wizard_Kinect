using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private int element;                //Current spell casted by the enemy.

	public float spellVelocity = 1000f; //Spell speed of the enemy
	public Rigidbody enemy_spell;
	public Transform enemySpellTransform;
	private PlayerHealth playerHealthScript;
	private LookPlayer lookPlayerScript;

    // Use this for initialization
    void Awake() {
        element = Random.Range(1, SpellManager.getnElements()); //Initialize the element of the enemy randomly.
		playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
		lookPlayerScript = gameObject.GetComponentInChildren<LookPlayer> ();
    }

	void Start () {
		InvokeRepeating ("attack", 2, 5);
	}
	
	// Update is called once per frame
	void Update () { 
	}


    public int getElement()
    {
        return element;
    }

	public void attack(){
		if (!playerHealthScript.isKO ()) { //if the player is not dead and the player is spotted.
			if (lookPlayerScript.isPlayerSpotted ()) {
				Rigidbody spellInstance = Instantiate (enemy_spell, enemySpellTransform.position, enemySpellTransform.rotation) as Rigidbody;

				// We set the spell velocity vector to the player
				spellInstance.velocity = spellVelocity * enemySpellTransform.forward;
			}
		} else {
			Debug.Log ("Player is dead :(");
		}
	}


}
