using System;
using System.Collections.Generic;
using System.Collections.Concurrent;


namespace GameFramework
{
    public sealed class Loader
    {
        private static Dictionary<Type, Queue<IRefrence>> refrenceCollction = new Dictionary<Type, Queue<IRefrence>>();
        public static T Generate<T>() where T : IRefrence
        {
            return (T)Generate(typeof(T));
        }

        public static IRefrence Generate(Type type)
        {
            type.EnsureObjectRefrenceType<IRefrence>();
            if (!refrenceCollction.TryGetValue(type, out Queue<IRefrence> refrences))
            {
                refrences = new Queue<IRefrence>();
                refrenceCollction.Add(type, refrences);
            }
            if (refrences.Count > 0)
            {
                return refrences.Dequeue();
            }
            return (IRefrence)Activator.CreateInstance(type);
        }

        public static void Release(IRefrence refrence)
        {
            GameFrameworkException.IsNull(refrence);
            Type type = refrence.GetType();
            if (!refrenceCollction.TryGetValue(type, out Queue<IRefrence> refrences))
            {
                refrences = new Queue<IRefrence>();
                refrenceCollction.Add(type, refrences);
            }
            refrence.Release();
            refrences.Enqueue(refrence);
        }
    }
}