using UnityEngine;
using UnityEngine.Pool;

public class SingleShotLinearWeapon : RangedWeapon
{
    private Pool<Projectile> _projectilesPool;

    private void Awake(){
        _projectilesPool = new Pool<Projectile>(CreateProjectile,OnTakeFromPool,RangedWeaponData.magSize+1);
    }
    
    private protected override void Update() => base.Update();

    protected override void Shoot() => _projectilesPool.GetObject();

    Projectile CreateProjectile()
    {
        Projectile projectile =
            Instantiate
            (
                ProjectilePrefab,
                ProjectileStartPostition.position,
                Quaternion.LookRotation(GetBulletDirection(),Vector3.up)
            );
        projectile.Damage = WeaponData.damage;
        projectile.Instantiator = transform.root.gameObject;
        projectile.transform.parent = null;
        projectile.OnDestroyAfterImpact.AddListener(() => ReturnToPool(projectile));
        // projectile.OnDestroyAfterImpact.AddListener(() => _pool.Release(projectile));
        return projectile;
    }

    void ReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.HasAlreadyCollided = false;
    }

    void OnTakeFromPool(Projectile projectile)
    {
        SetOnPosition(projectile.gameObject);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * RangedWeaponData.velocity;
        CurrentAmmo--;
        TimeSinceLastShoot = 0f;
        projectile.gameObject.SetActive(true);
    }

    void SetOnPosition(GameObject projectile)
    {
        projectile.transform.SetPositionAndRotation
        (
            ProjectileStartPostition.position,
            Quaternion.LookRotation(GetBulletDirection(),Vector3.up)
        );
    }

    void DestroyFromPool(Projectile projectile)
    {
        Destroy(projectile);
    }

    Vector3 GetBulletDirection() => Sight.GetRandomPointOnRadius() - ProjectileStartPostition.position;
}