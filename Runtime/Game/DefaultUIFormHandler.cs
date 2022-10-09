using System;
using UnityEngine;

namespace GameFramework.Game
{
    public sealed class DefaultUIFormHandler : IUIFormHandler
    {
        public int layer => throw new NotImplementedException();

        public GameObject gameObject => throw new NotImplementedException();

        public GameObject GetChild(string name)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }
    }
}