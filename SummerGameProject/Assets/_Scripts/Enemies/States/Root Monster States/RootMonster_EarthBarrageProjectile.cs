using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMonster_EarthBarrageProjectile : MonoBehaviour
{
    private bool _launching = false;

    private float _speed;

    private Vector3 _direction;

    private Vector3 _endPosition;

    private RootMonster_EarthBarrageState _earthBarrageState;

    private bool _reachedGoalPos;
    public void Launch(RootMonster_EarthBarrageState earthBarrageState, float speed, Vector3 endPosition)
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Earth barrage collision!" + collision.gameObject.name);

        

        //Issue is I can't find the enemy that created this which kinda sucks.
        //Potentially we use a getter via the EarthBarrage state so we can grab our owner and make sure not to damage ourself.
        //Or we have a layer specific to enemies however, enemies being able to damage eachother might be a wanted mechanic.

        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            //The game object can be damaged! Awesome.
            Debug.Log("Hit obj has a IDamageable interface implemented.");

            if (collision.gameObject.tag.Equals("Player"))
            {
                Debug.Log("Player call");
                collision.gameObject.GetComponent<IDamageable>().TakeDamage();
            }


        }


    }


}
