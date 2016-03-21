using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private int element; //Elemento con el que está sintonizado este enemigo.

    // Use this for initialization
    void Awake() {
        element = Random.Range(0, SpellManager.getnElements()); //Inicializa de forma aleatoria el elemento con el que está sintonizado el objetivo.
    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    public int getElement()
    {
        return element;
    }


}
