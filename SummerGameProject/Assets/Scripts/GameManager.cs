using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SceneObjects sceneObjects;
}

[Serializable] public class SceneObjects
{
    public GameObject player;
}
