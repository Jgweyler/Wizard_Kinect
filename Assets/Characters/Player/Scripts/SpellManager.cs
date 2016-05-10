using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public static int currentElement = 1; //Por defecto empezamos con fuego.
	public static int selectedElement = 1; //Especifica que elemento fue seleccionado por voz en Kinect
    private static int nElements = 6; //Numero de elementos con los que puede sintonizar el jugador.
    private CastSpell castSpellScript;
	private Image ElementImage; // Imagen del canvas que indica a que elemento actual está sintonizado el jugador.
    private static Rigidbody spell;                 //Contendrá el prefab del hechizo.
    private static Transform spellTransform;        //Posición donde se crearán los diferentes hechizos.
	private Image [] elementsSelection; //Conjunto de imagenes del canvas que permiten visualizar el elemento seleccionado.
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

		//Saca todas las imagenes y extrae la que te interesa.!!!!!
		Image [] imagenes = GameObject.FindGameObjectWithTag ("HealthUI").GetComponentsInChildren<Image>();

		ElementImage = imagenes [2];
		elementsSelection = new Image [nElements];

		for (int i = 1; i <= nElements; i++){
			elementsSelection[i -1] = imagenes[2 +i];
			if (i != 1) {
				elementsSelection [i - 1].CrossFadeAlpha (0f, 0.2f, true);
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
        //Atender a que elemento está sintonizado el personaje
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

        // Seteamos su velocidad hacia la misma trayectoria que hacia donde está mirando el personaje.
        spellInstance.velocity = currentCastForce * spellTransform.forward;
    }

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
		//Si el personaje no se está moviendo, permitimos que cambie de elemento.
		if (playerAnimator.GetBool ("isMoving") == false) {
			playerAnimator.Play ("SummonElement");
			elementsSelection [currentElement - 1].CrossFadeAlpha (0f, 0.2f, true);
			currentElement = element;
			elementsSelection [currentElement - 1].CrossFadeAlpha (1f, 0.2f, true);
			ElementImage.sprite = sprites [currentElement - 1]; // -1 porque se indexa desde cero.
			if (playerAnimator.GetBool ("hasTarget") == false)
				playerAnimator.Play ("idle");
			else
				playerAnimator.Play ("combatPos");
		}
	}

	public void summonElementKinect(int element){
		//Si el personaje no se está moviendo, permitimos que cambie de elemento.
		if ((playerAnimator.GetBool ("isMoving") == false) && (currentElement != selectedElement)) {
			playerAnimator.Play ("SummonElement");
			currentElement = element;
			ElementImage.sprite = sprites [currentElement - 1]; // -1 porque se indexa desde cero.
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
