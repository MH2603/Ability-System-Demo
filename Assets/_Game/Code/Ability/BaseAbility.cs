using Foundation.EntitySystem;
using MH.Character;
using MH.Skill;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace MH.Ability
{
    public class AbilityContext
    {
        public Vector3 TargetPosition { get; set; }
        public Transform Target { get; set; }
        public Vector3 Direction { get; set; }
        // Add other contextual data needed by abilities
    }

    public interface IAbility
    {
        string Name { get; }
        float Cooldown { get; }
        float DmgRadius { get; }
        float AttackLimitMax { get; }
        bool IsReady { get; }
        Action OnEnter { get; set; }    
        Action OnExit { get; set; }    
        void Initialize(EntityBase entity);
        void Execute(AbilityContext context);
        void Cancel();
        void UpdateAbility();

    }

    public abstract class BaseAbility<T> : IAbility where T : BaseAbilitySO
    {
        public Action OnEnter { get; set; }
        public Action OnExit { get ; set; }

        public string Name { get; protected set; }
        public float Cooldown => GetFinalAbilityStat(EAbilityStat.CoolDown);
        public float DmgRadius => GetFinalAbilityStat(EAbilityStat.DmgRadius);
        public float AttackLimitMax => GetFinalAbilityStat(EAbilityStat.AttackRadius);

        public bool IsReady => Time.time >= LastCastTime + Cooldown;

        protected EntityBase Entity;
        protected float LastCastTime;
        protected StatsMediator StatsMediator;
        protected T Config;

        protected Transform Root;
        protected PointContainer PointContainer;

        public BaseAbility(BaseAbilitySO config)
        {
            Name = config.AbilityName;
            this.Config = config as T;
        }

        
        public virtual void Initialize(EntityBase entity)
        {
            Entity = entity;
            StatsMediator = new StatsMediator();    

            foreach (var stat in Config.Stats)
            {
                StatsMediator.TryAddStat(stat); 
            }

            Root = entity.transform;

            PointContainer = entity.Get<PointContainer>();
        }

        public virtual void Execute(AbilityContext context)
        {
            if (!IsReady) return;
            LastCastTime = Time.time;
            OnEnter?.Invoke();
            OnExecute(context);
        }

        protected abstract void OnExecute(AbilityContext context);
        public virtual void Cancel() { }
        public virtual void UpdateAbility() { }

        protected float GetFinalAbilityStat(EAbilityStat statType)
        {
            return StatsMediator.GetFinalStatValue(AbilityStatTypeString.Convert(statType));
        }
    }
}
