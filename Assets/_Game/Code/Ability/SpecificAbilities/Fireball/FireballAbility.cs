using Cysharp.Threading.Tasks;
using Foundation.EntitySystem;
using MH.Character;
using MH.Projectile;
using System;
using UnityEngine;
using UnityEngine.Pool;

namespace MH.Ability
{
    public class FireBallAbility : BaseAbility<FireBallConfigSO>
    {
        protected IObjectPool<BaseProjectile> projectilePool;
        protected ProjectileContext projectileContext;
        protected Transform firePoint;

        protected float executeDuration => 0.5f;
        protected float dmg => GetFinalAbilityStat(EAbilityStat.Dmg);

        protected float projectileSpeed => GetFinalAbilityStat(EAbilityStat.ProjectileSpeed);
        //protected float projectileRotateSpeed => statsMediator.GetFinalStatValue(AbilityStatNames.ProjectileRotateSpeed);

        public FireBallAbility(BaseAbilitySO config) : base(config)
        {
            projectilePool = new ObjectPool<BaseProjectile>
                            (
                                createFunc: () => CreateNewProjectile(), // Create new instances
                                actionOnGet: null, // Action when taking from pool
                                actionOnRelease: obj => RelaeseProjectile(obj), // Action when returning to pool
                                actionOnDestroy: null, // Action when pool capacity is exceeded
                                collectionCheck: true, // Check for duplicates when returning to pool
                                defaultCapacity: 10, // Initial capacity
                                maxSize: 20 // Maximum pool size
                            );
                            
        }

        public override void Initialize(EntityBase entity)
        {
            base.Initialize(entity);

            firePoint = PointContainer.GetPoint(PointNames.FirePoint).Point;
        }

        protected override void OnExecute(AbilityContext context)
        {
            Shoot(context);
            DelayEndAbility();
        }

        protected BaseProjectile CreateNewProjectile()
        {
            BaseProjectile newProjectile = GameObject.Instantiate(Config.ProjectilePrefab);
            newProjectile.gameObject.SetActive(false);
            newProjectile.RegisterOnHitEvent(OnProjectileHit);
            return newProjectile;   
        }

        protected void RelaeseProjectile(BaseProjectile projectile)
        {
            projectile.ResetRuntimeProperties();
            projectile.gameObject.SetActive(false);
        }

        private void Shoot(AbilityContext context)
        {
            projectileContext = projectileContext.SetSpeed(projectileSpeed).
                                              //SetRotationSpeed(projectileRotateSpeed).
                                              SetCanHitLayer(Config.TargetLayer).
                                              //SetTarget(context.Target).
                                              SetDirection(context.Direction);

            BaseProjectile projectile = projectilePool.Get();
            projectile.transform.position = firePoint.position;
            projectile.gameObject.SetActive(true);
            projectile.TransitionData(projectileContext);

            projectile.Shoot();
        }

        private void OnProjectileHit(BaseProjectile hitProjectile, GameObject hitter)
        {
            if (hitter.GetComponent<CharacterHealth>())
            {
                hitter.GetComponent<CharacterHealth>().TakeDamage( Entity,dmg);
                projectilePool.Release(hitProjectile);
            }
        }

        private async void DelayEndAbility()
        {
            await UniTask.Delay((int)(executeDuration * 1000f));

            OnExit?.Invoke();
        }
    }
}
