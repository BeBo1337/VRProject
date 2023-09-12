using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    public GameObject muzzleFlashPrefab;
    private Scene currentScene; 
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private float destroyTimer = 2f;

    private const string mainMenuSceneName = "MainMenu";

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && GameManager.Instance.IsPlaying)
        {
            //Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger("Fire");
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
            // reset the bullet trail to return to pool
            var trail = newBullet.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.Clear();
            }
            newBullet.SetPosition(barrelLocation.position);
            newBullet.ShootInDirection(barrelLocation.transform.forward);
        }
    }
}
