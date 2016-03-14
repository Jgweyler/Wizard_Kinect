using UnityEngine;
using System.Collections;

public class MakeTarget : MonoBehaviour {

    private bool hasTarget;        //Si el jugador ha fijado un objetivo.
    private bool clickedTarget = false; //Determina si el usuario ha clickado/realizado el boton/gesto de fijar objectivo
    private GameObject target;     //Referencia al enemigo que ha marcado el jugador.
                                   // Use this for initialization
    public GameObject [] enemies;  //Referencia de todos los enemigos en el juego.
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        pointTarget();
	}

    private void pointTarget(){ //Fija el objetivo
        //Puedes usar la funcion de área de daño de los tanques, pero con los enemigos. Solo podrías seleccionar aquellos que sean enemigos.
        //Si se vuelve a pulsar el boton (o a realizar el evento de marcado), se debería cambiar de objetivo siempre y cuando lo haya.
        if (Input.GetMouseButton(1)){ //Devuelve si se pulso el click derecho.
            
        }
    }

    private void searchForEnemies(){ //Busca los enemigos que tenga en sus proximidades para marcar a uno.
    }
}
