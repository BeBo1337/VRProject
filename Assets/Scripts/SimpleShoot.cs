using UnityEngine;
using Managers;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    public GameObject muzzleFlashPrefab;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private float destroyTimer = 2f;
    
    private const string fire1String = "Fire1";
    private const string fireString = "Fire";

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown(fire1String) && GameManager.Instance.IsPlaying)
        {//Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger(fireString);
        }
    }


    //This function creates the bullet behavior
    void Shoot()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (muzzleFlashPrefab)
            {
                //Create the muzzle flash
                GameObject tempFlash;
                tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

                //Destroy the muzzle flash effect
                Destroy(tempFlash, destroyTimer);
            }

            // Creating a bullet and adding force to it in direction of the barrel
            var newBullet = BulletSpawner.BaseInstance.Spawn();
            newBullet.SetPosition(barrelLocation.position);
            newBullet.ShootInDirection(barrelLocation.transform.forward);
        }
    }
}
