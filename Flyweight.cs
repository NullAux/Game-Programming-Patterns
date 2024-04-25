using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyweight : MonoBehaviour
{
    //Flyweight Pattern. See Nystrom, R. (2014), Chapter 1 - Flyweight

    //The goal is to split a repeatedly instanced object in two - one part for the contextless, common
    //attributes shared by all (eg textures), and one for instance specific info (eg position), to make
    //each instance very light.
    //Nystrom gives examples of graphics rendering (minimizing data sent to gpu) and a tile map (keeping tile info in one reusable obj)
    
    //This example will look at models, sharing one model across many objects to (hopefully) create a neat graphics
    //effect with minimal work. Additionally, since all the instances share a set of properties, I'll experiment with
    //having them change from one central script, as opposed to Unity prefabs each with their own scripts.

    //The info shared between instances:
    //Nystrom (in C++) has a pointer to the shared info in each instance: For C#, I'm using a static class
    static class SharedInfo
    {
        //I'll use Unity's graphics objects-
        static Color SharedColor;
        static float SharedScale;
        public static Sprite SharedSprite;
        public static GameObject BaseGameObject;
        //^ Unity's Instantiate method returns a clone of a GameObject, so rather than each instance have two GOs (one in code
        //and one instantiated in scene) the sharedInfo gets a base one to copy. Unfortunately, component details aren't
        //copied, so they need to be set manually at each instances init
        public static GameObject player;//Effects will follow player
        //Note above public should really be private with return methods. I'm keeping them public for simplicity's sake.

        static SharedInfo()
        {
            player = GameObject.Find("Player");//Not very robust...
            BaseGameObject = new GameObject();
            BaseGameObject.SetActive(false);
            SharedSprite = Resources.Load<Sprite>("MySprite");
        }

        //-and a method to change it.
        //Nystrom notes that usually, a method here would be const and the shared data immutable, since you don't want to make changes
        //that would appear in multiple places. In this case, that's the point.
        static public void UpdateSharedModel(int currentFrame)
        {
            //We'll use Sin and the current frame to change the model over time
            float timeModifier = Mathf.Sin((currentFrame % 360f) * Mathf.PI * (1f / 180f));
            SharedScale = 0.5f + timeModifier * 0.5f;//Lock it to 0.5-1, to spare my eyes.
            timeModifier /= 0.5f;//Lock it to -0.5 to 0.5, so it doesn't push our RGB values out of range
            SharedColor = new Color(0.5f + timeModifier, 0.5f - timeModifier, 0.5f * Mathf.Abs(timeModifier));
        }

        //And a methods to get the model info...
        //As Nystrom points out, every instance will have to chase a pointer to get this info, every frame,
        //making this limited in use as a graphics effect. I'm not sure if using static will be better or worse...
        static public void UpdateInstance(ref SpriteRenderer spriteRenderer)
        {
            spriteRenderer.color = SharedColor;
            spriteRenderer.transform.localScale = Vector3.one * SharedScale;
        }

    }

    //Each instance
    class InstanceInfo
    {
        GameObject instance;
        SpriteRenderer spriteRenderer;
        Vector2 position;


        //Set up the instance
        //This code could really be in SharedInfo, to make InstanceInfo *really* slim
        public InstanceInfo(int instancesInstantiated) 
        {
            //Let's draw a spiral!
            float spiralConstant = (instancesInstantiated * Mathf.PI) % 2;
            position = new Vector2(Mathf.Cos(spiralConstant), Mathf.Sin(spiralConstant)) * instancesInstantiated * 0.5f;
            //Note. This isn't actually a spiral, it's a fan-like shape. However, this isn't a geometry project, so I'll give myself a pass.
            instance = Instantiate(SharedInfo.BaseGameObject, position, SharedInfo.player.transform.rotation, SharedInfo.player.transform);
            spriteRenderer = instance.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SharedInfo.SharedSprite;
            instance.SetActive(true);
            SharedInfo.UpdateInstance(ref spriteRenderer);
        }

        //Wrapper method for update call.
        //Bad for performance, makes update (below) more readable, since method is called directly on instance
        public void UpdateThisInstance()
        {
            SharedInfo.UpdateInstance(ref spriteRenderer);
        }

    }


    //Finally, generate and update instances over time.
    //Attatch this script to a GameObject, and it will create instances in the scene
    int instancesInstantiated = 0;//Used to offset each instance
    InstanceInfo[] instancesArray = new InstanceInfo[20];//Hold each instance
    float timeElapsed;

    private void Start()
    {
        instancesInstantiated = 0;
        timeElapsed = Time.time;
    }

    void Update()
    {
        //Update the shared model
        SharedInfo.UpdateSharedModel(Time.frameCount);

        //Update each instance
        foreach (InstanceInfo instance in instancesArray)
        {
            if (instance == null) { break; }
            instance.UpdateThisInstance();
        }

        //Add a new instance every 3 seconds
        if (Time.time - timeElapsed > 3 && instancesInstantiated < 20)
        {
            instancesInstantiated++;
            instancesArray[instancesInstantiated - 1] = new InstanceInfo(instancesInstantiated);
            timeElapsed = Time.time;
        }
    }
}
