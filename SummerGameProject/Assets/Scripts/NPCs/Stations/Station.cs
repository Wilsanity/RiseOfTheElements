using UnityEngine;

namespace Kibo.NPCs.Stations
{
    public class Station : MonoBehaviour
    {
        [SerializeField] [Min(1e-3f)] private float radius = 1f;
        [SerializeField] [Min(1f)] private float actionTime = 3f;

        public float Radius => radius;
        public float ActionTime => actionTime;
    }
}