
using System;
using System.Collections;
using DataTypes;
using Framework;
using Managers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _forceMultiplier;
    [SerializeField] private float _timeout;

    protected const string EnemyArm = "EnemyArm";
    protected const string EnemyLeg = "EnemyLeg";
    protected const string EnemyTorso = "EnemyTorso";
    protected const string EnemyHead = "EnemyHead";
    private void OnEnable()
    {
        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(_timeout);
        BulletSpawner.BaseInstance.Release(this);
    }

    public void ShootInDirection(Vector3 direction)
    {
        if (_rigidbody == null)
        {
            return;
        }

        var forceVector = direction * _forceMultiplier;
        _rigidbody.AddForce(forceVector, ForceMode.Impulse);
        
        AudioManager.Instance.PlaySoundEffect(SoundType.Shoot);
    }

    public void SetPosition(Vector3 worldPosition)
    {
        _rigidbody.MovePosition(worldPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitType bodyPartHit = hitEnemyWhere(other.gameObject.tag);
        if (bodyPartHit != HitType.Invalid)
        {
            Enemy enemy = FindEnemyInHierarchy(other.gameObject);
            if (enemy != null)
            {
                GameManager.Instance.BulletHitEnemy(this, enemy, bodyPartHit);
            }
        }
    }

    private Enemy FindEnemyInHierarchy(GameObject gameObject)
    {
        Transform currentTransform = gameObject.transform;
    
        while (currentTransform != null)
        {
            Enemy enemyComponent = currentTransform.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                return enemyComponent;
            }
        
            currentTransform = currentTransform.parent;
        }
    
        return null; // No Enemy component found in the hierarchy.
    }

    private HitType hitEnemyWhere(string tag)
    {
        switch (tag)
        {
            case EnemyArm:
                return HitType.LimbShot;
            case EnemyLeg:
                return HitType.LimbShot;
            case EnemyTorso:
                return HitType.BodyShot;
            case EnemyHead:
                return HitType.HeadShot;
            default:
                return HitType.Invalid;
        }
    }

}