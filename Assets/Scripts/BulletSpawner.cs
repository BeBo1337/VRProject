using Framework;
using UnityEngine;

public class BulletSpawner : Spawner<Bullet>
{
    public static BulletSpawner Instance { get; private set; }

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        base.Awake();
    }

    protected override void OnObjectRelease(Bullet obj)
    {
        // reset the moving Rigidbody
        var rigidBody = obj.gameObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.velocity = new Vector3(0f, 0f, 0f);
            rigidBody.angularVelocity = new Vector3(0f, 0f, 0f);   
        }
        base.OnObjectRelease(obj);
    }
}