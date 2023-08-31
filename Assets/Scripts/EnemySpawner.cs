using Framework;

public class EnemySpawner : Spawner<Enemy>
{
    public static EnemySpawner Instance { get; private set; }
    
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
}