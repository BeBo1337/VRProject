using System.Collections;
using DataTypes;
using Managers;
using UnityEngine;

//Bullet Logic
public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _forceMultiplier;
    [SerializeField] private float _timeout;

    // Constants for enemy body part tags
    protected const string EnemyArm = "EnemyArm";
    protected const string EnemyLeg = "EnemyLeg";
    protected const string EnemyTorso = "EnemyTorso";
    protected const string EnemyHead = "EnemyHead";
    
    private TrailRenderer trailRenderer;
    private void OnEnable()
    {
        StartCoroutine(Timeout()); // Start a coroutine for bullet timeout
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(_timeout); // Wait for the specified timeout duration
        BulletSpawner.BaseInstance.Release(this); // Release the bullet
    }

    // Function to shoot the bullet in a specified direction
    public void ShootInDirection(Vector3 direction)
    {
        if (_rigidbody == null)
        {
            return;
        }
        var forceVector = direction * _forceMultiplier; // Calculate the shooting force vector
        _rigidbody.AddForce(forceVector, ForceMode.Impulse); // Apply the force to the bullet
        AudioManager.Instance.PlaySoundEffect(SoundType.Shoot);
    }

    // Function to set the position of the bullet in the world
    public void SetPosition(Vector3 worldPosition)
    {
        _rigidbody.MovePosition(worldPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Determine which body part of an enemy was hit based on its tag
        HitType bodyPartHit = hitEnemyWhere(other.gameObject.tag);

        // Check if a valid body part was hit
        if (bodyPartHit != HitType.Invalid)
        {
            Enemy enemy = FindEnemyInHierarchy(other.gameObject);
            if (enemy != null)
            {
                BulletSpawner.BaseInstance.Release(this); // Return the bullet to the pool.
                // Inform the GameManager that the bullet hit an enemy and provide the details
                GameManager.Instance.BulletHitEnemy(enemy, bodyPartHit);
            }
        }
    }

    // Function to find an Enemy component in the hierarchy of a game object
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

    // Function to determine the hit type based on an enemy's body part tag
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
                return HitType.Invalid; // Return Invalid if the tag doesn't match any body part
        }
    }

}