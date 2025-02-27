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

    

    //We hit something, etc. (Could be a trigger if we wanted...
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.gameObject);


        IDamageable temp;
        if (!collision.gameObject.TryGetComponent<IDamageable>(out temp))
        {
            Debug.Log("We can't damage a non damageable object!");
            return;
        }

        //Patch solution... 
        //Need to use owner to deal damage to player 
        //I don't entirely like this. Since yes the root monster is the one dealing damage, however we have no information on what exactly kills the player
        //Like It'd be cool if we could rather pass the instigator as like the specific projectile maybe?

        if (collision.gameObject == _earthBarrageState.getOwner().gameObject)
        {
            Debug.Log("Can't hurt ourself with our own projectile....");
        }
        else
        {
            _earthBarrageState.getOwner().DealDamage(collision.gameObject, 1);
        }
    }

}
