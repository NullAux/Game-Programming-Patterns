using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    //A basic 2D control scheme.

    //--Variables
    //-Editor
    [SerializeField] float moveSpeed;

    //-Internal
    MyActions myActions;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize Command Pattern's actions class
        myActions = new MyActions(gameObject.GetComponent<Rigidbody2D>());

        
    }

    // Update is called once per frame
    void Update()
    {
        //Move player
        transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0);

        TestCommandPattern();
    }

    //--Command Pattern:

    //-Implement own actions.
    //Needs to be initialized in Start() with all the components its actions use.
    class MyActions : CommandPattern.CommandableActions
    {
        Rigidbody2D rigidbody;

        public MyActions(Rigidbody2D iRigidbody) { rigidbody = iRigidbody; }

        //For player, 'Move' will be an upward push, using its Rigidbody2D component
        public override void Move()
        {
            Debug.Log("I, the player, am moving!");
            rigidbody.AddForce(Vector2.up * 200);
        }

        //And their Announcement will be...
        public override void Announcement()
        {
            Debug.Log("I, the player, am announcing myself! Hyah!");
        }
    }

    //Call the commands for a test
    //Z to 'move', X to 'annonuce', C to 'RemoveGameObject'
    void TestCommandPattern()
    {
        CommandPattern.CommandClass myCommand = null;
        bool isAction = false;//< This exists because there's no way to tell if a command is for the gameObject or its action list.
        //The command itself could specify this when called and ask for the right parameter, but that adds duplicated code
        //at both ends.

        if (Input.GetKeyDown(KeyCode.Z))
        {
            myCommand = new CommandPattern.MoveAction();
            isAction = true;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            myCommand = new CommandPattern.AnnouncementAction();
            isAction = true;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            myCommand = new CommandPattern.RemoveGameObject();
        }

        //Execute command
        if (myCommand != null)
        {
            if (isAction)
            {
                myCommand.ExecuteAction((CommandPattern.CommandableActions)myActions);
            }
            else
            {
                myCommand.Execute(gameObject);
            }
        }



    }

}
