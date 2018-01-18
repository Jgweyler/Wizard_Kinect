using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public static int currentElement = 1;  //Set FIRE spell as default.
	public static int selectedElement = 1; //Set the selected Spell in the HUD
    private static int nElements = 6;      //Number of elements.
    private CastSpell castSpellScript;     //Reference to the cast spell script
	public Image ElementImage;             // Reference to the choosed element in the HUD (Left Image from the HP bar).
    private static Rigidbody spell;                 //Rigidbody reference of the spell.
    private static Transform spellTransform;        //Position where the spell is going to spawn.
	public Image [] elementsSelection;              //Images that represent the element selection of the player
	public Sprite [] sprites;

	private Animator playerAnimator;

	public static int FIRE = 1;
	public static int WATER = 2;
	public static int THUNDER = 3;
	public static int STONE = 4;
	public static int WIND = 5;
	public static int ICE = 6;

    private const int fire = 1;
	private const int water = 2;
	private const int thunder = 3;
	private const int stone = 4;
	private const int wind = 5;
	private const int ice = 6;

    void Awake()
    {
        castSpellScript = GetComponent<CastSpell>();
        spell = castSpellScript.spell;
        spellTransform = castSpellScript.spellTransform;
		playerAnimator = GetComponent<Animator> ();
    }
	void Start () {

        //Make invisibles all images that are not selected (Remember that is FIRE (0) by default

        for(int i = 0; i < nElements; i++)
        {
            if(i != 0)
            {
                elementsSelection[i].CrossFadeAlpha(0f, 0.2f, true);
            }
        }

	}
	
	// Update is called once per frame
	void Update () {
        switchElementListener();
    }

    public static void changeSpell(int element)
    {
        if(element > 0 && element <= nElements)
        {
            currentElement = element;
        }
    }

    public static void launchSpell(float currentCastForce)
    {
        //Check selected element by the player
        Renderer rend = spell.GetComponent<Renderer>();
        Spell spellScript = spell.GetComponent<Spell>();
        switch (currentElement)
        {
            case fire: rend.sharedMaterial.SetColor("_Color", Color.red); break;
            case water: rend.sharedMaterial.SetColor("_Color", Color.blue);break;
            case thunder: rend.sharedMaterial.SetColor("_Color", Color.yellow); break;
            case stone: rend.sharedMaterial.SetColor("_Color", Color.grey); break;
            case wind: rend.sharedMaterial.SetColor("_Color", Color.white); break;
            case ice: rend.sharedMaterial.SetColor("_Color", Color.cyan); break;
            default: break;
        }
        Rigidbody spellInstance = Instantiate(spell, spellTransform.position, spellTransform.rotation) as Rigidbody;

        // Set velocity vector of the spell..
        spellInstance.velocity = currentCastForce * spellTransform.forward;
    }

    //Handles the element changing (Keys: 1,2,3,4,5,6)
    private void switchElementListener() //
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
			summonElement (FIRE);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
			summonElement (WATER);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
			summonElement (THUNDER);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
			summonElement (STONE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
			summonElement (WIND);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
			summonElement (ICE);
        }
    }

	public void summonElement(int element){
        ////Change element only when the player is not moving
        if (playerAnimator.GetBool ("isMoving") == false) {
			playerAnimator.Play ("SummonElement");
			elementsSelection [currentElement - 1].CrossFadeAlpha (0f, 0.2f, true);
			currentElement = element;
			elementsSelection [currentElement - 1].CrossFadeAlpha (1f, 0.2f, true);
			ElementImage.sprite = sprites [currentElement - 1]; // -1 cos index starts from 0.
			if (playerAnimator.GetBool ("hasTarget") == false)
				playerAnimator.Play ("idle");
			else
				playerAnimator.Play ("combatPos");
		}
	}

    //KINECT!!!!!
	public void summonElementKinect(int element){
		//Change element only when the player is not moving
		if ((playerAnimator.GetBool ("isMoving") == false) && (currentElement != selectedElement)) {
			playerAnimator.Play ("SummonElement");
			currentElement = element;
			ElementImage.sprite = sprites [currentElement - 1];  // -1 cos index starts from 0.
            if (playerAnimator.GetBool ("hasTarget") == false)
				playerAnimator.Play ("idle");
			else
				playerAnimator.Play ("combatPos");
		}
	}
	public void switchElement_Kinect(){
		switch(selectedElement){
		case fire:
			summonElementKinect (fire);
			break;
		case water:
			summonElementKinect (water);
			break;
		case thunder:
			summonElementKinect (thunder);
			break;
		case wind:
			summonElementKinect (wind);
			break;
		case ice:
			summonElementKinect (ice);
			break;
		case stone:
			summonElementKinect (stone);
			break;
		default:
			break;
		}
	}

    public static int getCounterElement(int element)
    {
        int itsCounter = -1;

        switch (element)
        {
            case fire: itsCounter = water; break;
            case water: itsCounter = thunder; break;
            case thunder: itsCounter = stone; break;
            case stone: itsCounter = wind; break;
            case wind: itsCounter = ice ; break;
            case ice: itsCounter = fire; break;
            default: break;
        }

        return itsCounter;
    }
    
    public static int getnElements()
    {
        return nElements;
    }
}
