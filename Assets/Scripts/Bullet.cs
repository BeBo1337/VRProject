
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

    private void OnCollisionEnter(Collision other)
    {
        var hitAnEnemy = other.gameObject.CompareTag(GameManager.Instance.EnemyTag);
        if (hitAnEnemy)
        {
            GameManager.Instance.BulletHitEnemy(this, other.gameObject.GetComponent<Enemy>());
        }
    }
}