using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public static int currentElement = 1; //Por defecto empezamos con fuego.
    private static int nElements = 6; //Numero de elementos con los que puede sintonizar el jugador.

    private CastSpell castSpellScript;
    private static Rigidbody spell;                 //Contendrá el prefab del hechizo.
    private static Transform spellTransform;        //Posición donde se crearán los diferentes hechizos.

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
    }
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        SpellManager.switchElementListener();
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

    public static void switchElementListener() //
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentElement = fire;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentElement = water;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentElement = thunder;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentElement = stone;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentElement = wind;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentElement = ice;
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
