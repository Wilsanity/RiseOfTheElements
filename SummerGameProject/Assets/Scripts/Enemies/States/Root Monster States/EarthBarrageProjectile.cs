using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBarrageProjectile : MonoBehaviour
{
    private bool _launching = false;

    private float _speed;

    private Vector3 _direction;

    private Vector3 _endPosition;

    private EarthBarrageState _earthBarrageState;

    private bool _reachedGoalPos;
    public void Launch(EarthBarrageState earthBarrageState, float speed, Vector3 endPosition)
    {
        _reachedGoalPos = false;

        _endPosition = endPosition;
        _endPosition += Random.insideUnitSphere;

        _direction = _endPosition - transform.position;
        _direction.Normalize();


        _speed = speed;

        _earthBarrageState = earthBarrageState;

        _launching = true;

    }

    private void Update()
    {
        if (!_launching) return;

        transform.position += _direction * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _endPosition) > 0.5f) return;

        if(_reachedGoalPos == false)
        {
            _reachedGoalPos = true;
            _launching = false;
            _earthBarrageState.ProjectilesReachedGoal++;

            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_endPosition, 1f);
    }
}
