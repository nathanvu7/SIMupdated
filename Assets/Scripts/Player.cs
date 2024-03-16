using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;


public class Player : MonoBehaviour
{
    //[SerializeField] Transform spawnPoint;

    //Physics Stats
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float movementSpeed;
    [SerializeField] float boostSpeed;
    [SerializeField] float currentSpeed;
    [SerializeField] float turnSpeed;

    //Controls
    [SerializeField] Vector2 inputVectors;
    

    //Enemy Bot
    bool toggleEnemy = true;
    [SerializeField] GameObject enemy;


    //vars
    float accelInputs = 0;
    float steerInputs = 0;
    float rotationAngle = 0;

    //for multiplayer
    [SerializeField] int index = 0; //unique id for each bot, 0 = p1, 1 = p2, 2 = AI
    private GameObject organizerObj;
    private GameOrganizer organizer;

    //for Health and Damage system
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] GameObject explosion;
    bool state = true;

    void Start()
    {
        //this.transform.rotation = spawnPoint.rotation;

        currentSpeed = movementSpeed;
        organizerObj = GameObject.FindWithTag("Organizer");
        organizer = organizerObj.GetComponent<GameOrganizer>();

        organizer.UpdateScores(); //idk how to make organizer call this when a scene restarts (not onEnable somehow) so this'll do.
    }

    // Update is called once per frame
    void Update()
    {
        QuitGame();
        if (combatSystem.DeathCheck() == true) 
        {

            state = false;
            Instantiate(explosion, this.transform);
            enabled = false;
            organizer.PlayerLoses(index);
        }
    }

   

    void FixedUpdate()
    {
        AccelSystem();
        SteerSystem();
        if (organizer.StopSignal() == true)
        {
            enabled = false;
        }

    }

    public int GetPlayerIndex()
    {
        return index;
    }


    public void SetInputs(Vector2 inputVector)
    {
        inputVectors = inputVector; //passing it into a seperate vector so i can see in on inspection idk
        steerInputs = inputVectors.x;
        accelInputs = inputVectors.y;
    }

    void AccelSystem()
    {
        //transform.up = fwd //accel and steer inputs is 0/1 boolean dictating when a force is applied
        Vector2 engineForceVector = transform.up * accelInputs * currentSpeed;
        rb.AddForce(engineForceVector, ForceMode2D.Impulse);
    }
    void SteerSystem()
    {
        rotationAngle -= steerInputs * turnSpeed;
        rb.MoveRotation(rotationAngle);
    }

    

    void OnorOff()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            toggleEnemy = !toggleEnemy;
            enemy.SetActive(toggleEnemy);

        }
    }

    public void Boost2(float boost)
    {

        if (boost == 1)
        {
            //Debug.Log("pluh!2");
            currentSpeed = boostSpeed;
            combatSystem.SetBoost(true);
        }
        else
        {
            currentSpeed = movementSpeed;
            combatSystem.SetBoost(false);
        }
    }

    public bool GetState()
    {
        return state;
    }
    


    void QuitGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
            Debug.Log("Quitting game... ");
        }
    }




}
