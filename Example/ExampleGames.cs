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
        IEntity[] entities = Context.GetEntities(typeof(GameObjectComponent));
        //Debug.Log("running" + entities?.Length);
    }

    public void Release()
    {
    }
}
public class ExampleGames : MonoBehaviour
{

    public ResourceModle ResouceModle;

    void Start()
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
            SimpleWorld simpleWorld = Runtime.GetGameModule<WorldManager>().OpenWorld<SimpleWorld>();
            simpleWorld.AddScriptble<MovementScriptble>();
            for (var i = 0; i < 1000000; i++)
            {
                IEntity entity = simpleWorld.CreateEntity();
                entity.AddComponent<GameObjectComponent>();
            }
        });
    }
}



public sealed class SimpleWorld : AbstractGameWorld
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
}
