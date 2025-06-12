using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GunSystem : MonoBehaviour, IPausable
{
    [Header("Gun Stats")]
    public int damage;
    public LayerMask EnemyLayer;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public bool isPumpAction;

    [Header("Bools")]
    bool shooting, readyToShoot, reloading;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public ObjectPool bulletHolePool;

    [Header("Graphics")]
    public GameObject muzzleFlash;
    public GameObject bulletHolePrefab;
    public GameObject enemyImpactPrefab;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    [Header("ADS (Aiming)")]
    public bool allowADS = true;
    public float adsFOV = 40f;
    public float adsSpeed = 10f;
    private float defaultFOV;
    private bool aiming;

    private Coroutine recargaCoroutine;

    [Header("Recoil")]
    public float recoilCamera = 2f;
    public PlayerCam playerCam;

    [Header("Trail Settings")]
    public Material trailMaterial;
    public float trailSpeed = 200f;
    public float trailStartWidth = 0.05f;
    public float trailEndWidth = 0.01f;
    public float trailDuration = 0.2f;

    [Header("Animaciones")]
    public GameObject weaponAnimation;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        defaultFOV = fpsCam.fieldOfView;

        if (bulletHolePool == null)
            bulletHolePool = ObjectPool.Instance;

        if (bulletHolePool == null)
            Debug.LogWarning($"GunSystem en {gameObject.name} no encontró ObjectPool.Instance en Start.");
    }

    private bool isPaused = false;
    public void OnPause() => isPaused = true;
    public void OnResume() => isPaused = false;

    private void Update()
    {
        if (isPaused) return;
        MyInput();
        HandleADS();
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (Input.GetKeyDown(KeyCode.Mouse0) && reloading && isPumpAction && bulletsLeft > 0)
        {
            CancelInvoke(nameof(ReloadPumpAction));
            reloading = false;
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            //
            Animator anim = weaponAnimation.GetComponent<Animator>();
            anim.SetTrigger("Shoot");
            //
            Shoot();
        }
    }

    private void Shoot()
    {
        if (reloading)
        {
            if (recargaCoroutine != null)
            {
                StopCoroutine(recargaCoroutine);
                recargaCoroutine = null;
            }
            reloading = false;
        }

        readyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        Vector3 hitPoint;
        bool hitSomething = Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range);

        if (hitSomething)
        {
            hitPoint = rayHit.point;

            if (((1 << rayHit.collider.gameObject.layer) & EnemyLayer) != 0)
            {
                HealthControllerEnemy enemyHealth = rayHit.collider.GetComponent<HealthControllerEnemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);

                    if (enemyImpactPrefab != null)
                    {
                        bulletHolePool.GetPooledObject(enemyImpactPrefab, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    }
                }

                goto EndShoot;
            }

            // Impacto con superficie (pared, etc.)
            if (bulletHolePrefab != null)
            {
                bulletHolePool.GetPooledObject(bulletHolePrefab, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }
        else
        {
            hitPoint = fpsCam.transform.position + direction.normalized * range;
        }

        EndShoot:
        StartCoroutine(SpawnRuntimeTrail(attackPoint.position, hitPoint, !hitSomething));

        if (camShake != null)
            StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        if (muzzleFlash != null)
        {
            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation, attackPoint);
            Destroy(flash, 1f);
        }

        bulletsLeft--;

        if (playerCam != null)
            playerCam.ApplyRecoil(recoilCamera);

        bulletsShot--;

        Invoke(nameof(ResetShot), timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);


        //StartCoroutine(FinishAnimation());
    }

    /*private IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(timeBetweenShots); // Esperar el tiempo entre disparos

        Animator anim = weaponAnimation.GetComponent<Animator>();
        anim.SetBool("HasShoot", false); // Desactivar animación después del tiempo
    }*/

    private IEnumerator SpawnRuntimeTrail(Vector3 start, Vector3 end, bool isAirShot)
    {
        GameObject trailObj = new GameObject("BulletTrail");
        trailObj.transform.position = start;

        TrailRenderer trail = trailObj.AddComponent<TrailRenderer>();
        trail.time = trailDuration;
        trail.startWidth = trailStartWidth;
        trail.endWidth = trailEndWidth;
        trail.material = trailMaterial;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.alignment = LineAlignment.View;
        trail.autodestruct = false;
        trail.Clear();

        float distance = Vector3.Distance(start, end);
        float duration = distance / trailSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            trailObj.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        trailObj.transform.position = end;

        yield return new WaitForSeconds(isAirShot ? 10f : trail.time);

        Destroy(trailObj);
    }

    private void ResetShot() => readyToShoot = true;

    private void Reload()
    {
        if (reloading) return;
        reloading = true;

        if (recargaCoroutine != null)
            StopCoroutine(recargaCoroutine);

        recargaCoroutine = isPumpAction ? StartCoroutine(ReloadPumpAction()) : StartCoroutine(ReloadFinished());
    }

    private IEnumerator ReloadPumpAction()
    {
        while (bulletsLeft < magazineSize)
        {
            yield return new WaitForSeconds(reloadTime);
            if (!gameObject.activeInHierarchy || !reloading) yield break;
            bulletsLeft += bulletsPerTap;
            if (bulletsLeft >= magazineSize)
            {
                bulletsLeft = magazineSize;
                break;
            }
        }
        reloading = false;
    }

    private IEnumerator ReloadFinished()
    {
        yield return new WaitForSeconds(reloadTime);
        if (!gameObject.activeInHierarchy) { reloading = false; yield break; }
        bulletsLeft = magazineSize;
        reloading = false;
    }

    private void HandleADS()
    {
        if (!allowADS) return;

        aiming = Input.GetKey(KeyCode.Mouse1);
        float targetFOV = aiming ? adsFOV : defaultFOV;
        fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, targetFOV, Time.deltaTime * adsSpeed);
    }

    private void OnDisable()
    {
        if (recargaCoroutine != null)
        {
            StopCoroutine(recargaCoroutine);
            recargaCoroutine = null;
            reloading = false;
        }
    }

    public void ForzarCargadorLleno()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
