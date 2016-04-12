using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeTarget : MonoBehaviour {

    private bool hasTarget;        //Si el jugador ha fijado un objetivo.
    private bool isPointing;       //Indica si se está señalando o designando un objetivo
    private GameObject target;     //Referencia al enemigo que ha marcado el jugador.
    private List<GameObject> enemies;  //Referencia de todos los enemigos en el juego.

    private GameManager gameManagerScript;
    private PlayerMovement playerMovementScript;

    void Awake() {
        hasTarget = false;
    }
    void Start () {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerMovementScript = gameObject.GetComponent<PlayerMovement>();
        enemies = gameManagerScript.getEnemies();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1) && !hasTarget){ //Si pulsa el botón y además no tiene objetivo asignado, lo intentamos asignar
            pointTarget();
        }
	}

    private void pointTarget(){ //Fija el objetivo
        //Puedes usar la funcion de área de daño de los tanques, pero con los enemigos. Solo podrías seleccionar aquellos que sean enemigos.
        //Si se vuelve a pulsar el boton (o a realizar el evento de marcado), se debería cambiar de objetivo siempre y cuando lo haya.
        SphereCollider player_range = gameObject.GetComponent<SphereCollider>();
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i] !=null && player_range.bounds.Contains(enemies[i].transform.position)){ //Si el enemigo está dentor del rango del jugador, se marca como objetivo.
                addTarget(enemies[i]);
                return;
            }
        }
    }

    private void searchForEnemies(){ //Busca los enemigos que tenga en sus proximidades para marcar a uno.
    }

    public void addTarget(GameObject tg)
    {
        hasTarget = true;
        target = tg;
        playerMovementScript.setHasTarget(hasTarget);
        playerMovementScript.setTarget(tg);
    }

    public void deleteTarget()
    {
        hasTarget = false;
        target = null;
        playerMovementScript.setHasTarget(hasTarget);
        playerMovementScript.setTarget(null);
    }
}
