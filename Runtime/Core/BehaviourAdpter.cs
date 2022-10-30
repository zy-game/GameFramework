using UnityEngine;

namespace GameFramework
{
    public sealed class BehaviourAdpter : MonoBehaviour
    {
        public GameFrameworkAction update, fixedUpdate, lateUpdate, applictionQuit, destory;
        private void Update()
        {
            if (update == null)
            {
                return;
            }
            update();
        }

        private void FixedUpdate()
        {
            if (fixedUpdate == null)
            {
                return;
            }
            fixedUpdate();
        }

        private void LateUpdate()
        {
            if (lateUpdate == null)
            {
                return;
            }
            lateUpdate();
        }

        private void OnApplicationQuit()
        {
            if (applictionQuit == null)
            {
                return;
            }
            applictionQuit();
        }

        private void OnDestroy()
        {
            if (destory == null)
            {
                return;
            }
            destory();
        }
    }
}