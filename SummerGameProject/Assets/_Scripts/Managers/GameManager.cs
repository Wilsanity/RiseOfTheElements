using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : AdvancedFSM
{
    public SceneObjects sceneObjects;

    public Slider slider = null;

    protected override void Initialize()
    {
        ConstructFSM();
    }

    protected override void FSMUpdate()
    {
        elapsedTime += Time.deltaTime;
        CurrentState.Reason(sceneObjects.player.transform, transform);
        CurrentState.Act(sceneObjects.player.transform, transform);
    }

    private void ConstructFSM()
    {
        // Create States
        //
        // Create Start State
        GM_StartState start = new GM_StartState(slider, sceneObjects);

        // Create Pause State
        GM_PauseState pause = new GM_PauseState(slider);

        // Create End State
        GM_EndState end = new GM_EndState(slider, sceneObjects);

        // Create Play State
        GM_PlayState play = new GM_PlayState(slider);

        // Add Transitions
        //
        // Transitions out of Start state
        start.AddTransition(TransitionType.Pausing, FSMStateType.Pause);
        start.AddTransition(TransitionType.Ending, FSMStateType.End);
        start.AddTransition(TransitionType.Playing, FSMStateType.Play);

        // Transitions out of Pause state
        pause.AddTransition(TransitionType.Starting, FSMStateType.Start);
        pause.AddTransition(TransitionType.Ending, FSMStateType.End);
        pause.AddTransition(TransitionType.Playing, FSMStateType.Play);

        // Transitions out of End State
        end.AddTransition(TransitionType.Starting, FSMStateType.Start);
        end.AddTransition(TransitionType.Pausing, FSMStateType.Pause);
        end.AddTransition(TransitionType.Playing, FSMStateType.Play);

        // Transitions out of Play State
        play.AddTransition(TransitionType.Starting, FSMStateType.Start);
        play.AddTransition(TransitionType.Pausing, FSMStateType.Pause);
        play.AddTransition(TransitionType.Ending, FSMStateType.End);

        // Add States to List
        //
        AddState(start);
        AddState(pause);
        AddState(end);
        AddState(play);
    }
}

[Serializable] public class SceneObjects : IEnumerable<GameObject>
{
    public GameObject player;
    public List<GameObject> testCube;

    //I added these so that I can call on the SceneObjects in other states - Luka
    public IEnumerator<GameObject> GetEnumerator()
    {
        if (player != null)
        {
            yield return player;
        }

        if(testCube != null)
        {
            foreach(var cube in testCube)
            {
                yield return cube;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}
