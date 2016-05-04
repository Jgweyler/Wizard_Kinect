using UnityEngine;
using System.Collections;

public class CastSpell : MonoBehaviour {

    private string spellButton;              // Botón usado para lanzar los hechizos.
    private float currentCastForce;         // Fuerza con la que será lanzado el hechizo (para que cuando recorra el mapa se mueva mas rápido o más lento).
    public float minCastForce = 15f;        //Fuerza mínima para lanzamiento de hechizos
    public float maxCastForce = 30f;
    public float maxChargeTime = 0.75f;     //Máximo tiempo en el que se puede aumentar/castear la fuerza del hechizo. (Manteniendo pulsado el boton de lanzamiento del mismo)
    private float chargeSpeed;
	private Animator playerAnimator;
    private bool casted;                    //Indica si un hechizo ya fue lanzado.
	private bool casted_kinect;             //Indica si el hechizo ha sido cargado previamente con Kinect
    public Rigidbody spell;                 //Contendrá el prefab del hechizo.
    public Transform spellTransform; //Posición donde se crearán los diferentes hechizos.
    //El mago dispone de diferentes tipos de hechizo

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
        // Cuando el jugador entra en juego, debemos settear su fuerza minima de casteo.
        currentCastForce = minCastForce;
    }

    // Update is called once per frame
    void Update () {

		if (Input.GetButtonDown (spellButton)) {
			playerAnimator.SetBool ("isCasting", true);
		} else {
			playerAnimator.SetBool ("isCasting", false);
		}
        
        // Si se ha alcanzado la fuerza máxima de casteo y el hechizo no se ha lanzado...
        if (currentCastForce >= maxCastForce && !casted)
        {
            // ... usamos la máxima fuerza y lanzamos el hechizo.
            currentCastForce = maxCastForce;
			//playerAnimator.SetBool ("isLaunching", true);
			//playerAnimator.PlayInFixedTime("launchSpell");
			playerAnimator.Play ("launchSpell");
            Fire();
        }
        // Si no, si el botón de casteo ha empezado a ser pulsado...
        else if (Input.GetButtonDown(spellButton))
        {
            // ... reseteamos el disparador de casteo y la fuerza del mismo.
            casted = false;
            currentCastForce = minCastForce;
        }
        //Sino, Si el botón/movimiento sique pulsado/realizandose y el hechizo no ha sido lanzado
        else if (Input.GetButton(spellButton) && !casted)
        {
            // Incrementamos la fuerza de casteo
            currentCastForce += chargeSpeed * Time.deltaTime;

        }
        //Si no, si el botón//movimiento de casteo ha sido presionado/realizado y el hechizo no ha sido lanzado
        else if (Input.GetButtonUp(spellButton) && !casted)
        {
            // ... lanzamos el hechizo.
			//playerAnimator.SetBool ("isLaunching", true);
			//playerAnimator.PlayInFixedTime("launchSpell");
			playerAnimator.Play ("launchSpell");
			Debug.Log (playerAnimator.GetCurrentAnimatorStateInfo(0));
            Fire();
        }


    }
	/*
	IEnumerator animateLaunch(){
		while (playerAnimator.is) { // or whatever this property is called
			yield return null;
		}
	}*/


    private void Fire()
    {
        // Seteamos el disparador de casteo, así solo se lanzará el hechizo una vez
        casted = true;

        SpellManager.launchSpell(currentCastForce);
        // Volvemos a resetear la fuerza de lanzamiento del hechizo.
        currentCastForce = minCastForce;
		//playerAnimator.SetBool ("isLaunching", false);
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


