using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public float maxHealth = 100f;
    public float currentHealth;
    public float flashSpeed = 5f; //Velocity wich the damage Image flashes (When the player receive damage).
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); //Set color red the damage image.

    private PlayerMovement playerMovement;                              // Reference to playerMovement script.
    private CastSpell playerCastSpell;                                  // Reference to CastSpell script..
    private GameObject HUDCanvas;                                       //HUD reference.
    private Slider HealthSlider;                                        //HP health bar of the player
    private Image damageImage;                                          //Damage Image. -> Appears when the player receives damage
	private Animator playerAnimator;									//Animator reference to handle all the animations of the player.

    private bool isDead;                                                // Checks if player is dead
    private bool damaged;                                               // Checks if player receives damage.

    void Awake() {
        //Set the current health to MAX.
        currentHealth = maxHealth;
        isDead = false;
        damaged = false;
    }
	// Use this for initialization
	void Start () {
        //Set all necessary references.
        HUDCanvas = GameObject.FindGameObjectWithTag("HUDCanvas");  //Las referencias al HUDCanvas están en la función Start porque en Awake el HUDCanvas no ha sido
        damageImage = HUDCanvas.GetComponentInChildren<Image>();    //instanciado todavía.
        HealthSlider = HUDCanvas.GetComponentInChildren<Slider>();

        playerMovement = GetComponent<PlayerMovement>();
        playerCastSpell = GetComponentInChildren<CastSpell>();
		playerAnimator = GetComponent<Animator> ();
    }
	
	// Update is called once per frame
	void Update () {
        // If the player has been damaged
        if (damaged)
        {
            // ... Set the flash color the de DamageImage.
            damageImage.color = flashColour;
        }
        // Si no
        else
        {
            // ... we set the color of the screen to normal.
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
        //The player is dead :( -> Set bool and dead animation. Disable scripts.
        isDead = true;
		playerAnimator.Play ("Die");

        //Desactivamos los scripts de ataque y movimiento.
        playerMovement.enabled = false;
        playerCastSpell.enabled = false;
    }

	public bool isKO(){
		return isDead;
	}
}
