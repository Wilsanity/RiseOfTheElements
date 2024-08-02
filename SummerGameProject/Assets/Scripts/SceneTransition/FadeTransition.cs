
using UnityEngine;

public class FadeTransition : MonoBehaviour, ISceneTransition
{
    [Header("Scene Transition Animation")]
    [SerializeField] private AnimationClip enterSceneAnimation;
    [SerializeField] private AnimationClip exitSceneAnimation;
    public AnimationClip EnterSceneAnimation { get { return enterSceneAnimation; } }

    public AnimationClip ExitSceneAnimation { get { return exitSceneAnimation; } }


}
