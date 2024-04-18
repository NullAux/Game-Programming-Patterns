using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPattern : MonoBehaviour
{
    //Implementing Command Pattern. See Chapter 2: Command
    //Nystrom's example includes a 'GameActor' class, which can accept commands by calling GameActor methods.
    //As Unity's GameObjects and related components already does this, rather than create a second generic class
    //I'm making two kinds of commands:
    //1. Pass in a GameObject, call a GameObject or guranteed component method (eg transform)
    //2. Have a class of overridable actions. Pass in a GameObject's version of this action class, call one of its override methods.

    //--Commands (base)
    //Base class. This is almost identical to Nystrom's example, with C# keywords and my action version of Execute.
    //Any Command instance should implement for either GameObjects or Actions
    public abstract class CommandClass
    {
        //No deconstructor since this is C#
        public virtual void Execute(GameObject actor)
        {
            Debug.Log("Base CommandClass GameObject Execute called.");
        }

        public virtual void ExecuteAction(CommandableActions actions)
        {
            Debug.Log("Base CommandClass Action Execute called.");
        }

    }

    //NullCommand for debug / passing to prevent crashes
    class NullCommand : CommandClass
    {
        public override void Execute(GameObject actor)
        {
            Debug.Log("NullCommand called (GameObject)");
        }

        public override void ExecuteAction(CommandableActions actions)
        {
            Debug.Log("NullCommand called (Action)");
        }
    }

    //--Class containing 'actions' (generic behaviour not defined by GameObject).
    //Any script can make its own instance and override the methods to be appropriate to itself. This allows Commands
    //to call generic actions which are implemented differently by the reciever (eg 'move').

    //I spent some time trying to make a seperate Action class, with one Action Command which could accpet an action
    //(subchild of Action, eg 'move') and then find the GameObject/Script's implementation of that action (the subchild of that subchild), but
    //couldn't get the last step working. Could be worth revisiting, but given that actions are meant to be generic,
    //having everything that implements actions have its own CommandableActions obj with the full list isn't so bad.

    public class CommandableActions
    {
        //Can add any behaviours wanted here (in a GameObjects instance)
        //This lets you do things like GetComponent() safely, since it's implemented within the script of the GameObject
        //that uses it.

        //These have to be public to let Commands see them. Would a friend class work in C++ better?
        public virtual void Move() { }
        public virtual void Announcement() { }

    }

    //--Commands
    //-GameObject Commands
    public class RemoveGameObject : CommandClass
    {
        public override void Execute(GameObject actor)
        {
            Debug.Log(actor.name + " is being removed!");
            actor.SetActive(false);
        }
    }

    //-Action Commands
    //These all follow the same pattern. See above comments on trying to get a generic Action Command.
    //Behaviour is implemented in different scripts via CommandableAction method overrides.
    public class MoveAction : CommandClass
    {
        public override void ExecuteAction(CommandableActions actions)
        {
            actions.Move();
        }
    }

    public class AnnouncementAction : CommandClass
    {
        public override void ExecuteAction(CommandableActions actions)
        {
            actions.Announcement();
        }
    }

}
