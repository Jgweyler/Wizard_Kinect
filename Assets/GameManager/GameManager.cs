using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private const float Y_POS = 10f;    //Max Y axis for spawning enemies .
    private const float MIN_POS = 1.0f; //Min X and Z for spawning enemies.
    public int numEnemies;

    public GameObject player;       //Player Object
    public GameObject playerSpell;  //Spell Object
    public GameObject enemy;        //Prefab of an enemy.
    public GameObject terrain;      //Game terrain.
    public GameObject main_camera;  //Main camera.
    public GameObject hudCanvas;    //HUD

    private  List<GameObject>enemies;   //All enemies in the game

    void Awake(){
        Instantiate(terrain, terrain.transform.position, terrain.transform.rotation); //Create the rettain.
        //Instantiate(player, player.transform.position, player.transform.rotation);   
        //Instantiate(hudCanvas, hudCanvas.transform.position, hudCanvas.transform.rotation);
        //hudCanvas = GameObject.Find("HUDCanvas");
        Instantiate(main_camera, main_camera.transform.position, main_camera.transform.rotation); //Create themain camera. 

        instanceEnemies();        //Create enemies.



    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    }

    private void instanceEnemies(){

        enemies = new List<GameObject>();

        Terrain terrainObject = terrain.GetComponent<Terrain>();
        Vector3 terrainSize = terrainObject.terrainData.size;

        float max_x = terrainSize.x;
        float max_z = terrainSize.z;

        Vector3 randomPos = Vector3.zero;
        //enemies = new GameObject[numEnemies];

        for (int i = 0; i < numEnemies; i++)
        {
            randomPos = new Vector3(Random.Range(MIN_POS, max_x), Y_POS, Random.Range(MIN_POS, max_z));
            //enemies[i] = Instantiate(enemy, randomPos, enemy.transform.rotation) as GameObject;
            GameObject enemigo = Instantiate(enemy, randomPos, enemy.transform.rotation) as GameObject;
            enemies.Add(enemigo);
        }
    }

    public List<GameObject> getEnemies(){
        return enemies;
    }
}
