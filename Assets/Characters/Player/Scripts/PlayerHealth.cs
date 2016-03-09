using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public float maxHealth = 100f;
    public float currentHealth;
    public Slider HealthSlider;
    public Image damageImage;
    public float flashSpeed = 5f; //Velocidad en la que flashea la imagen de daño (damagenImage) en la pantalla.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); //El color al que estará seteado la imagen de daño (damageImage) para flashear.

    PlayerMovement playerMovement;                              // Referencia al movimiento del personaje.
    CastSpell playerCastSpell;                                  // Referencia al casteo de hechizos del personaje.
    bool isDead;                                                // Si el personaje está muerto.
    bool damaged;                                               // Cuando el personaje recibe daño.

    void Awake() {
        //Inicializamos todas las referencias.
        playerMovement = GetComponent<PlayerMovement>();
        playerCastSpell = GetComponentInChildren<CastSpell>();

        //Ponemos la vida al máximo cuando se inicialice el script
        currentHealth = maxHealth;
        isDead = false;
        damaged = false;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Si el personaje ha sido dañado
        if (damaged)
        {
            // ... asociamos el color de (flashColour) a la imagen de daño para que se vea reflejado en la pantalla.
            damageImage.color = flashColour;
        }
        // Si no
        else
        {
            // ... transitamos para que el color de la pantalla vuelva a estar normal.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }

    public void TakeDamage(int amount){
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        HealthSlider.value = currentHealth;

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death();
        }
    }


    void Death(){
        //Indicamos que el personaje está muerto y por lo tanto, no podrá atacar ni moverse.
        isDead = true;

        //Desactivamos los scripts de ataque y movimiento.
        playerMovement.enabled = false;
        playerCastSpell.enabled = false;
    }
}
