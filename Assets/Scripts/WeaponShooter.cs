using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class WeaponShooter : MonoBehaviour
{

    [Tooltip("Projecttile for rpg and bullet for normal guns")]
    public enum ShooterType {BULLET,PROJECTILE }
    public ShooterType shooterType = ShooterType.BULLET;
    // muzzle flash settings
    [Header("Muzzle flash")]
    public ParticleSystem muzzleFlash;

    // sound settings
    [Header("Sound Settings")]
    public AudioClip fireSound;
    public AudioClip readySound;
    public AudioClip hideSound;
    public AudioClip reloadSound;
    private AudioSource weaponAudioSource;

    //animation settings
    [Header("Animation Settings")]
    private Animator animator;

    // shooter settings
    [Header("Shooter Settings")]
    public LayerMask hitLayer;
    [ShowIf("shooterType",ShooterType.BULLET)]public Bullet bullet;
    [ShowIf("shooterType", ShooterType.BULLET)] public int bulletPerShot = 1;
    public float fireDeltatime;
    [Range(0,0.1f)] public float defaultSpread;
    [ShowIf("shooterType", ShooterType.BULLET)] public float bulletDelay;
    public Vector3 defaultCamRecoil;
    private Vector3 camRecoil;
    private float previousFireTime=0;
    private GameObject decalprefab;
    private GameObject bulletImpact;
    private AudioClip impactSound;
    private Camera fpsCamera;
    private Vector3 fireDirection;
    private float spread;
    private delegate void ShooterFunctionsReference();
    private ShooterFunctionsReference ShooterFunction;

    // Projectile Settings
    [Header("Projectile Settings")]
    [ShowIf("shooterType",ShooterType.PROJECTILE)]public GameObject projectile;
    [ShowIf("shooterType", ShooterType.PROJECTILE)] public Transform projectileSpawnPoint;
    [ShowIf("shooterType", ShooterType.PROJECTILE)] public MeshRenderer rocket;
    [ShowIf("shooterType", ShooterType.PROJECTILE)] public float range;

    // Shell Ejecting Settings
    [Header("Shell Ejecting Settings")]
    [ShowIf("shooterType", ShooterType.BULLET)] public GameObject shellPrefab;
    [ShowIf("shooterType", ShooterType.BULLET)] public Transform shellSpawnPoint;
    [ShowIf("shooterType", ShooterType.BULLET)] public float ejectForce;

    // aim down sights settings
    [Header("Aim Settings")]
    public Vector3 aimOffset;
    public float aimDuration;
    public float aimCameraFOV;
    [Range(0, .01f)] public float aimSpread;
    public Vector3 aimCamRecoil;
    private bool isAiming = false;
    private Vector3 defaultPosition;
    private float defaultCameraFOV;
    private Coroutine routineReference;

    // Sniper Settings
    [Header("Sniper Settings")]
    [ShowIf("shooterType", ShooterType.BULLET)] public bool hasWeaponSniperScope;
    [ShowIf("hasWeaponSniperScope")] public GameObject scopeUI;
    [ShowIf("hasWeaponSniperScope")] public Camera weaponCamera;
    [ShowIf("hasWeaponSniperScope")] public float bulletSpeed;
    private bool scoped = false;

    // Trail settings
    [Header("Trail Settings")]
    [ShowIf("shooterType", ShooterType.BULLET)] public bool hasBulletTrail;
    [ShowIf("shooterType", ShooterType.BULLET)] public TrailRenderer trail;
    [ShowIf("shooterType", ShooterType.BULLET)] public Transform TrailSpawner;

    // Reload Settings
    [Header("Reload Settings")]
    public int magazineSize = 20;
    public int currentAmmoInMagazine = 0;
    public int reserveAmmo = 180;
    public bool inReloadingAnimation = false;
    private bool isReloading = false;
    

    // GUI settings
    [Header("GUI Settings")]
    public Sprite crossHairImage;
    public Sprite weaponIconImage;
    public string weaponName;
    private GameObject weaponGUI;
    private GameObject crossHair;
    private Text ammo;

    // camera recoil settings
    private RecoilController recoilController;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.keepAnimatorStateOnDisable = true;
        weaponAudioSource = GetComponent<AudioSource>();
        reserveAmmo -= magazineSize;
        currentAmmoInMagazine = magazineSize;
        crossHair = GameObject.FindGameObjectWithTag("crosshair");
        weaponGUI = GameObject.FindGameObjectWithTag("WeaponGUI");
        if (shooterType==ShooterType.BULLET)
        {
            if (bulletPerShot>1)
            {
                ShooterFunction = MultipleShooter;
            }
            else
            {
                if (hasBulletTrail)
                {
                    if (hasWeaponSniperScope)
                    {
                        ShooterFunction = SniperShooter;
                    }
                    else
                        ShooterFunction = TrailShooter;
                }
                else
                    ShooterFunction = RayShooter;
            }

        }
        else
        {
            ShooterFunction = ProjectileShooter;
        }
    }
    void Start()
    {
        
        weaponAudioSource.clip = fireSound;
        
        fpsCamera = Camera.main;
        bullet = Instantiate<Bullet>(bullet);
        defaultPosition = transform.localPosition;
        defaultCameraFOV = fpsCamera.fieldOfView;
        
        spread = defaultSpread;
        recoilController = GetComponentInParent<RecoilController>();
        camRecoil = defaultCamRecoil;
    }
    void OnEnable()
    {
        animator.SetBool("active", true);
        weaponAudioSource.PlayOneShot(readySound);
        WeaponGUIStart();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (Time.time - previousFireTime >= fireDeltatime && currentAmmoInMagazine >0 && !isReloading)
            {
                previousFireTime = Time.time;
                muzzleFlash.Play();
                weaponAudioSource.Play();
                animator.SetTrigger("fire");
                currentAmmoInMagazine--;
                fireDirection = fpsCamera.transform.forward + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
                var recoilVector = new Vector3(camRecoil.x, Random.Range(-camRecoil.y, camRecoil.y), Random.Range(-camRecoil.z, camRecoil.z));
                if (hasBulletTrail && ! hasWeaponSniperScope)
                {
                    recoilController.FireRecoil(recoilVector);
                    ShooterFunction();
                }
                else
                {
                    ShooterFunction();
                    recoilController.FireRecoil(recoilVector);
                }
                EjectingShell();
                

            }


        }
        else
        {
            previousFireTime = 0;
        }
        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            isAiming = !isAiming;
            if (routineReference!=null)
            {
                StopCoroutine(routineReference);
            }
            if (isAiming )
            {
                routineReference= StartCoroutine(AimDownSights(aimOffset,aimCameraFOV));
                animator.SetBool("aimed", true);
                spread = aimSpread;
                camRecoil = aimCamRecoil;
            }
            else
            {
                routineReference= StartCoroutine(AimDownSights(defaultPosition,defaultCameraFOV));
                animator.SetBool("aimed", false);
                spread = defaultSpread;
                camRecoil = defaultCamRecoil;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmoInMagazine < magazineSize || currentAmmoInMagazine <= 0)
        {
            if (!isReloading && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
        
    }

    private void EjectingShell()
    {
        if (shellPrefab == null || shellSpawnPoint == null)
            return;

        var shell = Instantiate(shellPrefab, shellSpawnPoint.position, shellSpawnPoint.rotation);
        var shellScript = shell.GetComponent<Shell>();
        shellScript.ejectForce = ejectForce;
        if (isAiming)
        {
            shell.transform.localScale *= aimCameraFOV / defaultCameraFOV;
            shellScript.ejectForce *= 0.7f;
        }
    }

    void LateUpdate()
    {
        WeaponGUIUpdate();
    }
    private void TrailShooter()
    {
        if (trail == null || TrailSpawner == null)
        {
            return;
        }
        RaycastHit hit;
        TrailRenderer trailRenderer = Instantiate(trail, TrailSpawner.position, TrailSpawner.rotation);
        trailRenderer.AddPosition(TrailSpawner.position);
        if (Physics.Raycast(fpsCamera.transform.position, fireDirection, out hit, bullet.range, hitLayer))
        {
            trailRenderer.transform.position = hit.point;
            if (bulletDelay == 0)
                bulletDelay = trailRenderer.time;
            StartCoroutine(BulletEffects(hit, bulletDelay));
        }
        else
        {
            trailRenderer.transform.position = fpsCamera.transform.position + fireDirection * bullet.range/10;
        }
    }
    private void SniperShooter()
    {
        if (trail==null || TrailSpawner==null)
        {
            return;
        }
        RaycastHit hit;
        TrailRenderer trailRenderer = Instantiate(trail, TrailSpawner.position, TrailSpawner.rotation);
        if (!scoped)
        {
            trailRenderer.emitting = false;
        }
        trailRenderer.AddPosition(TrailSpawner.position);
        if (Physics.Raycast(fpsCamera.transform.position, fireDirection, out hit, bullet.range, hitLayer))
        {
            trailRenderer.transform.position = hit.point;
            float distance = Vector3.Distance(TrailSpawner.position, hit.point);
            bulletDelay = distance / bulletSpeed;
            trailRenderer.time = bulletDelay;
            StartCoroutine(BulletEffects(hit, bulletDelay));
        }
        else
        {
            trailRenderer.transform.position = fpsCamera.transform.position + fireDirection * bullet.range / 10;
        }
    }
    private void RayShooter()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fireDirection, out hit, bullet.range, hitLayer))
        {
            if (bulletDelay == 0)
            {
                SpawnBulletFX(hit);
                SpawnForce(hit);
                SpawnDamage(hit);
            }
            else
              StartCoroutine(BulletEffects(hit, bulletDelay));
        }
    }
    private void ProjectileShooter()
    {
        rocket.enabled = false;
        Vector3 targetPosition;
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fireDirection, out hit, range, hitLayer))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = fpsCamera.transform.position + fireDirection * range;
        }
        Instantiate(projectile, projectileSpawnPoint.position, Quaternion.LookRotation(targetPosition - projectileSpawnPoint.position));
    }
    private void MultipleShooter()
    {
        for (int i = 0; i < bulletPerShot; i++)
        {
            RayShooter();
            fireDirection = fpsCamera.transform.forward + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        }
    }

    private void SpawnDamage(RaycastHit hit)
    {
        DamageHandler damageHandler = hit.transform.GetComponent<DamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.DamageReceiver(bullet.damage);
        }
    }

    private void SpawnForce(RaycastHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;
        if (rigidbody != null)
        {
            rigidbody.AddForceAtPosition(bullet.force * -hit.normal, hit.point);
        }
    }

    private void SpawnBulletFX(RaycastHit hit)
    {
        SurfaceIdentifier surfaceIdentifier = hit.transform.GetComponent<SurfaceIdentifier>();
        if (surfaceIdentifier != null)
        {
            decalprefab = surfaceIdentifier.decalprefab;
            bulletImpact = surfaceIdentifier.bulletImpact;
            impactSound = surfaceIdentifier.impactSound;
            surfaceIdentifier.audioSource.PlayOneShot(impactSound);
        }
        else
        {
            decalprefab = bullet.defaultDecal;
            bulletImpact = bullet.defaultImpact;
            impactSound = bullet.defaultImpactSound;
            bullet.transform.position = hit.transform.position;
            bullet.audioSource.PlayOneShot(impactSound);            
        }
        GameObject decal = Instantiate(decalprefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
        GameObject impact = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
        decal.transform.parent = hit.transform;
        Destroy(decal, 7);
        Destroy(impact, 5);
    }

    private IEnumerator BulletEffects(RaycastHit hit, float time)
    {
        yield return new WaitForSeconds(time);
        SpawnBulletFX(hit);
        SpawnForce(hit);
        SpawnDamage(hit);
    }
    
    private IEnumerator AimDownSights(Vector3 targetPosition,float targetCameraFOV)
    {
        OutScope();
        crossHair.SetActive(!crossHair.activeSelf);
        Vector3 startPosition = transform.localPosition;
        float startCameraFOV = fpsCamera.fieldOfView;
        float t = 0;
        float lerpDuration = aimDuration;
        float elapsedTime=0;
        while (elapsedTime<=lerpDuration)
        {
            t = elapsedTime / lerpDuration;
            if (!hasWeaponSniperScope)
                t = 1 - Mathf.Pow(1 - t, 4);
            Vector3 nextPosition= Vector3.Lerp(startPosition, targetPosition, t);
            transform.localPosition = nextPosition;

            // lerp camera FOV
            float nextCameraFOV = Mathf.Lerp(startCameraFOV, targetCameraFOV, t);
            fpsCamera.fieldOfView = nextCameraFOV;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
        fpsCamera.fieldOfView = targetCameraFOV;
        InScope();
    }
    /// <summary>
    ///  
    /// </summary>
    public void UnWield()
    {
        if (isAiming)
        {
            isAiming = false;
            routineReference = StartCoroutine(AimDownSights(defaultPosition, defaultCameraFOV));
            animator.SetBool("aimed", false);
            spread = defaultSpread;
            camRecoil = defaultCamRecoil;
        }
        animator.SetBool("active", false);
        weaponAudioSource.PlayOneShot(hideSound);
    }
    public IEnumerator Reload()
    {
        isReloading = true;
        if (currentAmmoInMagazine <= 0)
        {
            yield return new WaitForEndOfFrame();
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Aim Fire"))
            {
                yield return null;
            }
        }
        if (isAiming)
        {
            isAiming = false;
            routineReference = StartCoroutine(AimDownSights(defaultPosition, defaultCameraFOV));
            animator.SetBool("aimed", false);
            spread = defaultSpread;
            camRecoil = defaultCamRecoil;
        }
        animator.SetBool("reloading", true);
        inReloadingAnimation = true;
        weaponAudioSource.PlayOneShot(reloadSound);
        while (inReloadingAnimation)
        {
            yield return null;
        }
        
        int spentAmmo = magazineSize - currentAmmoInMagazine;
        if (reserveAmmo >= spentAmmo)
        {
            reserveAmmo -= spentAmmo;
            currentAmmoInMagazine += spentAmmo;
        }
        else
        {
            currentAmmoInMagazine += reserveAmmo;
            reserveAmmo = 0;
        }
        isReloading = false;
    }
    private void WeaponGUIStart()
    {
        crossHair.GetComponent<Image>().sprite = crossHairImage;
        if (weaponGUI!=null)
        {
            weaponGUI.transform.GetChild(0).gameObject.GetComponent<Text>().text = weaponName;
            weaponGUI.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = weaponIconImage;
            ammo = weaponGUI.transform.GetChild(2).gameObject.GetComponent<Text>();
        }
    }
    private void WeaponGUIUpdate()
    {
        ammo.text = currentAmmoInMagazine + " / " + reserveAmmo;
    }
    private void InScope()
    {
        if (!hasWeaponSniperScope || scopeUI==null || weaponCamera==null)
        {
            return;
        }
        if (isAiming)
        {
            weaponCamera.gameObject.SetActive(false);
            scopeUI.SetActive(true);
            scoped = true;
        }
    }
    private void OutScope()
    {
        if (!hasWeaponSniperScope || scopeUI == null || weaponCamera == null)
        {
            return;
        }
        if (!isAiming)
        {
            weaponCamera.gameObject.SetActive(true);
            scopeUI.SetActive(false);
            scoped = false;
        }
    }

    
}
