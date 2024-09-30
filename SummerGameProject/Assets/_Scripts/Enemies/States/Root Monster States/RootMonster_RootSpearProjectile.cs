using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMonster_RootSpearProjectile : MonoBehaviour
{
    private RootMonster_RootSpearsState _rootSpearState;
    private RootMonsterEnemyController _enemyController;
    private float _groundHeight;
    private int _spikeUp; //0 is idle, 1 is up, 2 is down
    private float _heightOfSpike;
    private bool _isRetracted = false;

    private void Awake()
    {
        _heightOfSpike = GetComponent<CapsuleCollider>().height;
    }

    public void Initialize(RootMonster_RootSpearsState rootSpearState, RootMonsterEnemyController enemyController, float groundHeight)
    {
        _spikeUp = 0;

        _isRetracted = false;

        _rootSpearState = rootSpearState;

        _enemyController = enemyController;

        _groundHeight = groundHeight;

        float randomDelayTime = Random.Range(_enemyController.RootSpearPopUpDelayMin, _enemyController.RootSpearPopUpDelayMax);

        Invoke(nameof(SpikeUp), randomDelayTime);
    }

    private void Update()
    {
        if(_spikeUp == 1)
        {
            if (transform.position.y > _groundHeight + _heightOfSpike / 4) return;

            transform.position += transform.up * _enemyController.RootSpearPopUpSpeed * Time.deltaTime;
        }
        else if (_spikeUp == 2) 
        {
            if (transform.position.y < _groundHeight - _heightOfSpike) { Retracted(); return; }

            transform.position -= transform.up * _enemyController.RootSpearPopUpSpeed * Time.deltaTime;
        }



    }
    public void SpikeUp()
    {
        _spikeUp = 1;
        

    }

    public void SpikeDown()
    {
        _spikeUp = 2;
    }


    private void Retracted()
    {
        if(!_isRetracted)
        {
            _isRetracted = true;

            _rootSpearState.SpikesRetracted++;
        }
    }

    //Delay For Random.Range between min and max

    //Transform.position upwards until we hit the ground
}
