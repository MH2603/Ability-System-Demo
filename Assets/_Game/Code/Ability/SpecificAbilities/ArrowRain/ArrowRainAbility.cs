
using Foundation.EntitySystem;
using UnityEngine;
using UnityEngine.Pool;

namespace MH.Ability
{
    public class ArrowRainAbility : BaseAbility<ArrowRainConfig>
    {
        private float dmg => GetFinalAbilityStat(EAbilityStat.Dmg);
        private float dmgDuration => GetFinalAbilityStat(EAbilityStat.DmgDuration); 

        private IObjectPool<AreaDamageInstance> areaDmgPool;

        public ArrowRainAbility(BaseAbilitySO config) : base(config)
        {
            
        }

        public override void Initialize(EntityBase entity)
        {
            base.Initialize(entity);
            SetUpPools();
        }

        protected override void OnExecute(AbilityContext context)
        {
            AreaDamageInstance areaDmg = areaDmgPool.Get();
            areaDmg.Init(CreateAreaContext());
            areaDmg.transform.position = context.TargetPosition;

            areaDmg.gameObject.SetActive(true);
            areaDmg.TriggerAreaDamage();

            OnExit?.Invoke();
        }

        private void SetUpPools()
        {
            areaDmgPool = new ObjectPool<AreaDamageInstance>
                            (
                                createFunc: () => CreateAreaDmg(), // Create new instances
                                actionOnGet: null, // Action when taking from pool
                                actionOnRelease: obj => obj.gameObject.SetActive(false), // Action when returning to pool
                                actionOnDestroy: null, // Action when pool capacity is exceeded
                                collectionCheck: true, // Check for duplicates when returning to pool
                                defaultCapacity: 10, // Initial capacity
                                maxSize: 20 // Maximum pool size
                            );
            
        }

        private AreaDamageInstance CreateAreaDmg()
        {
            AreaDamageInstance newAreaDmg = Object.Instantiate(Config.AreaDamagePrefab);
            newAreaDmg.gameObject.SetActive(false);

            return newAreaDmg;
        }

        private AreaDamageContext CreateAreaContext()
        {
            return new AreaDamageContext().
                        SetApplyDmgOverTime(true).
                        SetDMG(dmg).
                        SetRadius(DmgRadius).
                        SetPool(areaDmgPool).
                        SetTargetLayer(Config.TargetLayer).
                        SetDamageDuration(dmgDuration).
                        SetTickInterval(0.5f).
                        SetAttacker(Entity);
        }
    }
}
