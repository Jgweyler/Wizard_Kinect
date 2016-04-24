using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private int element; //Elemento con el que está sintonizado este enemigo.

	public float spellVelocity = 1000f; //Velocidad del hechizo enemigo
	public Rigidbody enemy_spell;
	public Transform enemySpellTransform;
	private PlayerHealth playerHealthScript;
	private LookPlayer lookPlayerScript;


    // Use this for initialization
    void Awake() {
        element = Random.Range(1, SpellManager.getnElements()); //Inicializa de forma aleatoria el elemento con el que está sintonizado el objetivo.
		playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
		lookPlayerScript = gameObject.GetComponentInChildren<LookPlayer> ();

    }

	void Start () {
		InvokeRepeating ("attack", 2, 5);
	}
	
	// Update is called once per frame
	void Update () { // No puedes llamarlo cada frame! Lo que estas haciendo es, para cada "frame" llamar la funcion 
					 //una burrada de veces! O_O
	}


    public int getElement()
    {
        return element;
    }

	public void attack(){
		if (!playerHealthScript.isKO ()) { //Si el jugador no esta muerto y está avistado
			if (lookPlayerScript.isPlayerSpotted ()) {
				Rigidbody spellInstance = Instantiate (enemy_spell, enemySpellTransform.position, enemySpellTransform.rotation) as Rigidbody;

				// Seteamos su velocidad hacia la misma trayectoria que hacia donde está mirando el enemigo.
				spellInstance.velocity = spellVelocity * enemySpellTransform.forward;
			}
		} else {
			Debug.Log ("El colega está muerto");
		}
	}


}
