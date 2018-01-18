using UnityEngine;
using System.Collections;

public class CastSpell : MonoBehaviour {

    private string spellButton;             // Button used to launch spells.
    private float currentCastForce;         // Force of the spell
    public float minCastForce = 15f;        // Min cast force
    public float maxCastForce = 30f;        // Max cast force
    public float maxChargeTime = 0.75f;     // Maximum time which the player can increment the force of the spell
    private float chargeSpeed;
	private Animator playerAnimator;
    private bool casted;                    //If a spell has been casted.
	private bool casted_kinect;             //If the spell has been casted with Kinect
    public Rigidbody spell;                 //Rigidbody of the spell.
    public Transform spellTransform;        //Position where the spells are going to be spawned
    //IMPORTANT: THE PLAYER HAS DIFFERENT TYPES OF SPELLS (FIRE, WATER, EARTH, THUNDER...)


    void Awake() {
		
    }

    // Use this for initialization
    void Start () {
		playerAnimator = GetComponent<Animator> ();
        chargeSpeed = (maxCastForce - minCastForce) / maxChargeTime;
        spellButton = "Fire1";
		casted_kinect = false;

    }
    private void OnEnable()
    {
        // Set the cast force to the minimum.
        currentCastForce = minCastForce;
    }

    // Update is called once per frame
    void Update () {
		
        // if the current cast force is maximum and the spell hasn't been thrown yet
        if (currentCastForce >= maxCastForce && !casted)
        {
            //We use the maximum force and throw the spell
            currentCastForce = maxCastForce;
			playerAnimator.Play ("launchSpell");
            Fire();
        }
        // Else, the player starts to cast a spell
        else if (Input.GetButtonDown(spellButton))
        {
            // ... reset our casted boolean and the current force to the min.
            casted = false;
            currentCastForce = minCastForce;
        }
        //Else, If the fire button has not been released, increment the cast force.
        else if (Input.GetButton(spellButton) && !casted)
        {
            // Incrementamos la fuerza de casteo
            currentCastForce += chargeSpeed * Time.deltaTime;

        }
        //Else, if the spell has not been thrown yet... fire!
        else if (Input.GetButtonUp(spellButton) && !casted)
        {
            // ... throw spell.
			playerAnimator.Play ("launchSpell");;
            Fire();
        }


    }

    private void Fire()
    {
        //set the casted value to true
        casted = true;

        SpellManager.launchSpell(currentCastForce);
        //reset the force of the spell
        currentCastForce = minCastForce;
    }

	public void setCasted_Kinect(bool cast_kinect)
	{
		casted_kinect = cast_kinect;
	}

	public bool getCasted_Kinect()
	{
		return casted_kinect;
	}
}


