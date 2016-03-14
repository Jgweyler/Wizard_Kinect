using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private const float Y_POS = 0.5f; //Posicion máxima de eje Y donde se colocarán los enemigos.
    private const float MIN_POS = 1.0f; //Posiciones mínimas de las coordenadas X y Z de los enemigos
    public int numEnemies;

    public GameObject player;       //Jugador
    public GameObject playerSpell;  //Hechizo del jugador
    public GameObject enemy;        //Prefab de los enemigos para poder instanciarlos.
    public GameObject terrain;      //Terreno sobre el que se desarrolla el juego.
    public GameObject main_camera;  //Camara del juego.
    public GameObject hudCanvas;    //Elementos gráficos de la pantalla (canvas)

    private GameObject [] enemies;   //Almacena todas las referencias de los enemigos en el juego.

    void Awake(){
        Instantiate(terrain, terrain.transform.position, terrain.transform.rotation); //Primero es necesario generar un terreno.
        Instantiate(player, player.transform.position, player.transform.rotation);    //Creamos al jugador.
        Instantiate(hudCanvas, hudCanvas.transform.position, hudCanvas.transform.rotation);
        Instantiate(main_camera, main_camera.transform.position, main_camera.transform.rotation); //Creamos la cámara del juego. 

        instanceEnemies();        //Generamos a los enemigos.



    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    }

    private void instanceEnemies(){
        Terrain terrainObject = terrain.GetComponent<Terrain>();
        Vector3 terrainSize = terrainObject.terrainData.size;

        float max_x = terrainSize.x;
        float max_z = terrainSize.z;

        Vector3 randomPos = Vector3.zero;
        enemies = new GameObject[numEnemies];

        for (int i = 0; i < numEnemies; i++)
        {
            randomPos = new Vector3(Random.Range(MIN_POS, max_x), Y_POS, Random.Range(MIN_POS, max_z));
            enemies[i] = Instantiate(enemy, randomPos, enemy.transform.rotation) as GameObject;
        }
    }
}
