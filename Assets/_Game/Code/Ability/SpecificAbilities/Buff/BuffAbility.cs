using Cysharp.Threading.Tasks;
using Foundation.EntitySystem;
using Foundation.Extensions.AttributeSystem;
using MH.Character;
using UnityEngine;
using UnityEngine.Pool;

namespace MH.Ability
{
    public class BuffAbility : BaseAbility<BuffAilityConfigSO>
    {

        #region ------------ Properties ------------
        private ObjectPool<GameObject> _vfxPool;

        private float buffDuration => GetFinalAbilityStat(EAbilityStat.DmgDuration);

        #endregion

        public BuffAbility(BaseAbilitySO config) : base(config)
        {

        }
        public override void Initialize(EntityBase entity)
        {
            base.Initialize(entity);

            foreach (var buffStat in Config.BuffStats)
            {
                StatsMediator.TryAddStat(buffStat);
            }

            SetUpVFXPool();
        }

        protected override void OnExecute(AbilityContext context)
        {
            CheckToApplyAllBuffForTarget(context);
            ApplyVfxForTagret(context.Target);
        }

        #region ---------- Aplly Buff -------

        private void CheckToApplyAllBuffForTarget(AbilityContext context)
        {
            EntityBase targetEntity = context.Target.GetComponent<EntityBase>();
            if (targetEntity == null)
            {
                Debug.Log(" BUG Buff Aility: Target is not Entity !");
                return;
            }

            CharacterStat targetStatCtrl = targetEntity.Get<CharacterStat>();
            if (targetStatCtrl == null)
            {
                Debug.Log(" BUG Buff Aility: Target not include CharacterStat !");
                return;
            }

            foreach (var buffStat in Config.BuffStats)
            {
                ApplyBuffForTarget(buffStat, targetStatCtrl);
            }
        }


        private void ApplyBuffForTarget(BuffStat buffStat, CharacterStat target)
        {
            IModifierOperation operation = null;
            if ( buffStat.OperationType == EModifierOperation.Multi )
            {
                operation = new MultiplyModifier(buffStat.BaseValue);
            }
            else
            {
                operation = new AddModifier(buffStat.BaseValue);
            }

            StatModifier modifier = StatModifierPool.Get(statName: buffStat.AffectStatName,
                                                         operation: operation,
                                                         duration: buffDuration,
                                                         source: Entity);

            target.AddModifier(modifier);   
        }

        #endregion

        #region ----------- VFx -----------------

        private void SetUpVFXPool()
        {
            _vfxPool = new ObjectPool<GameObject>(
                                createFunc: () => CreateVfx(),
                                actionOnGet: null,
                                actionOnRelease: obj => obj.SetActive(false),
                                actionOnDestroy: null,
                                collectionCheck: false,
                                defaultCapacity: 5,
                                maxSize: 20);
        }

        private GameObject CreateVfx()
        {
            GameObject vfx = Object.Instantiate(Config.BuffVfxPrefab);
            vfx.SetActive(false);

            return vfx; 
        }

        private void ApplyVfxForTagret(Transform target)
        {
            GameObject vfx = _vfxPool.Get();
            vfx.transform.SetParent(target.transform);
            vfx.transform.localPosition = Vector3.zero;

            vfx.SetActive(true);

            TimingOffVfx(vfx);
        }

        private async void TimingOffVfx(GameObject vfx)
        {
            await UniTask.Delay( (int)(buffDuration * 1000));
            _vfxPool.Release(vfx);
        }

        #endregion
    }
}
