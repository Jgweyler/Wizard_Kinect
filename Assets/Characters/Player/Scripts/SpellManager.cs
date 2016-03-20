using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    private static int currentElement = 1; //Por defecto empezamos con fuego.
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
            case fire: rend.sharedMaterial.SetColor("_Color", Color.red); spellScript.setElementType(fire); break;
            case water: rend.sharedMaterial.SetColor("_Color", Color.blue); spellScript.setElementType(water); break;
            case thunder: rend.sharedMaterial.SetColor("_Color", Color.yellow); spellScript.setElementType(thunder); break;
            case stone: rend.sharedMaterial.SetColor("_Color", Color.grey); spellScript.setElementType(stone); break;
            case wind: rend.sharedMaterial.SetColor("_Color", Color.white); spellScript.setElementType(wind); break;
            case ice: rend.sharedMaterial.SetColor("_Color", Color.cyan); spellScript.setElementType(ice); break;
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
            currentElement = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentElement = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentElement = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentElement = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentElement = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentElement = 6;
        }


    }
}
