using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Game;
using GameFramework.Resource;
public sealed class GameObjectComponent : IComponent
{
    public void Release()
    {

    }
}

public sealed class MovementScriptble : IGameScript
{
    public void Executed(IGameWorld world)
    {
    }

    public void Release()
    {
    }
}
public class ExampleGames : MonoBehaviour
{

    public ResourceModle ResouceModle;

    async void Start()
    {
        ResourceManager resourceManager = Runtime.GetGameModule<ResourceManager>();
        resourceManager.SetResourceModle(ResouceModle);
        resourceManager.CheckoutResourceUpdate("https://saltgame-1251268098.cos.ap-chengdu.myqcloud.com/",
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
            SimpleWorld simpleWorld = Runtime.GetGameModule<GameManager>().OpenWorld<SimpleWorld>();
            simpleWorld.AddScriptble<MovementScriptble>();
            IEntity entity = simpleWorld.CreateEntity();
            entity.AddComponent<GameObjectComponent>();
        });

        // await Task.Delay(5000);
        // Runtime.GetGameModule<GameFramework.Game.GameManager>().CloseWorld<SimpleWorld>();
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
        Test();
    }

    private void Test()
    {
        Debug.Log(name + " startboot");

    }

    public override void Update()
    {
    }
}
