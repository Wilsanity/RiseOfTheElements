using UnityEngine;

public interface ISceneTransition
{

    AnimationClip EnterSceneAnimation { get; }
    AnimationClip ExitSceneAnimation { get; }
    //Animation PlayEnterSceneTransition(); //plays the scene transition when we enter a scene
    //Animation PlayExitSceneTransition(); //plays the scene transition when we exit a scene
}