using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Game;
using GameFramework.Resource;

public class ExampleGames : MonoBehaviour
{
    public GameFramework.Resource.ResouceModle ResouceModle;
    async void Start()
    {
        ResourceManager resourceManager = Runtime.GetGameModule<ResourceManager>();
        resourceManager.SetResourceModle(ResouceModle);
        resourceManager.CheckoutResourceUpdate("",
        args =>
        {
            Debug.Log("resource update progres:" + args);
        },
        state =>
        {
            if (state != ResourceUpdateState.Success)
            {
                Debug.Log("update resource failur");
                return;
            }
            Runtime.GetGameModule<WorldManager>().OpenGame<SimpleWorld>();
        });
    }

    // Update is called once per frame
    void Update()
    {
        Runtime.Update();
    }

    private void OnApplicationQuit()
    {
        Runtime.Shutdown();
    }
}

public sealed class SimpleWorld : GameWorld
{
    public override string name => "SimpleWorld";

    public SimpleWorld()
    {

    }

    public override void Update()
    {
    }
}
