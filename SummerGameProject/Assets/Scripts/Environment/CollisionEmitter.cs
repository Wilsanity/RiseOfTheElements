using UnityEngine;

namespace Kibo.Environment
{
    public class CollisionEmitter : EnvironmentEmitter
    {
        private void OnCollisionEnter(Collision collision)
        {
            Emit();
        }
    }
}