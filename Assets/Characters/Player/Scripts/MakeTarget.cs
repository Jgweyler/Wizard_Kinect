using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MakeTarget : MonoBehaviour {

    private bool hasTarget;             //If the player has a target to attack.
    private bool isPointing;             //Indicates if the player is pointing an enemy (KINECT)
    public static GameObject target;     //Reference to the enemy targeted by the player.
	public static CanvasGroup enemyCanvasGroup; // Enemy HUD
	public static Slider enemyHealthSlider;     //Enemy health bar

    private List<GameObject> enemies;  //All enemies in the game.
	private Animator playerAnimator;

    private GameManager gameManagerScript;
    private PlayerMovement playerMovementScript;
	private Image enemyElement;

    void Awake() {
        hasTarget = false;
    }
    void Start () {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerMovementScript = gameObject.GetComponent<PlayerMovement>();
		playerAnimator = GetComponent<Animator> ();
        enemies = gameManagerScript.getEnemies();

		//Obtenemos las referencias de el slider que representa la vida del enemigo y la imagen que indica el elemento al que esta
		//Sintonizado
		enemyCanvasGroup = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponentsInChildren<Canvas>()[1].GetComponent<CanvasGroup>();
		enemyHealthSlider = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponentsInChildren<Slider>()[1];
		enemyElement = enemyCanvasGroup.GetComponentsInChildren<Image> ()[2];
		enemyCanvasGroup.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1) && !hasTarget){ //Press right click and the player will try to assign a target.
            pointTarget();
        }
	}

    private void pointTarget(){ //Points target (With right mouse button)

        SphereCollider player_range = gameObject.GetComponent<SphereCollider>();
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i] !=null && player_range.bounds.Contains(enemies[i].transform.position)){ //If the enemy is in range -> Assign that enemy as target.
                addTarget(enemies[i]);
                return;
            }
        }
    }

	public void pointTarget_Kinect(){ //ONLY WITH KINECT
		if (!hasTarget) { //Make sure that the player has no target alredy.
			SphereCollider player_range = gameObject.GetComponent<SphereCollider> ();
			for (int i = 0; i < enemies.Count; i++) {
				if (enemies [i] != null && player_range.bounds.Contains (enemies [i].transform.position)) { //If the enemy is in range -> Assign that enemy as target.
					addTarget (enemies [i]);
					return;
				}
			}
		}
	}

    public void addTarget(GameObject tg) //Called when the player makes an enemy a target
    {
        hasTarget = true;
		playerAnimator.SetBool ("hasTarget", true);
        target = tg;
        playerMovementScript.setHasTarget(hasTarget);
        playerMovementScript.setTarget(tg);
		enemyHealthSlider.value = tg.GetComponent<EnemyHealth> ().getCurrentHealth ();
		int spriteElement = tg.GetComponent<EnemyBehavior> ().getElement () - 1;
		enemyCanvasGroup.alpha = 1f; //Make visible enemy info (HP, Spell...)
		enemyElement.sprite = GetComponent<SpellManager>().sprites [spriteElement];
    }

    public void deleteTarget()
    {
        hasTarget = false;
		playerAnimator.SetBool ("hasTarget", false);
		playerAnimator.Play ("idle");
        target = null;
        playerMovementScript.setHasTarget(hasTarget);
        playerMovementScript.setTarget(null);
		enemyCanvasGroup.alpha = 0f;  //Make invisible enemy info (HP, Spell...)
    }

	public bool pointed_target(){ //Return the current target.
		return hasTarget;
	}

	public static void updateEnemySlider(float newValue){
		enemyHealthSlider.value = newValue;
	}
}
