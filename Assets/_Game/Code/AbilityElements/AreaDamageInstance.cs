using Foundation.EntitySystem;
using MH.Character;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace MH
{

    public class AreaDamageContext
    {
        public float DMG { get; private set; }
        public float Radius { get; private set; }
        public LayerMask TargetLayer { get; private set; }
        public bool ApplyDmgOverTime { get; private set; } // Enable damage over time
        public float DamageDuration { get; private set; } // Duration for damage over time
        public float TickInterval { get; private set; } // Time interval between damage ticks
        public IObjectPool<AreaDamageInstance> Pool { get; private set; }
        public EntityBase Attacker { get; private set; }

        /// <summary>
        /// Sets the damage amount.
        /// </summary>
        public AreaDamageContext SetDMG(float dmg)
        {
            DMG = dmg;
            return this;
        }

        /// <summary>
        /// Sets the radius of the damage.
        /// </summary>
        public AreaDamageContext SetRadius(float radius)
        {
            Radius = radius;
            return this;
        }

        /// <summary>
        /// Sets the layer mask for the damage targets.
        /// </summary>
        public AreaDamageContext SetTargetLayer(LayerMask targetLayer)
        {
            TargetLayer = targetLayer;
            return this;
        }

        /// <summary>
        /// Enables or disables damage over time.
        /// </summary>
        public AreaDamageContext SetApplyDmgOverTime(bool applyDmgOverTime)
        {
            ApplyDmgOverTime = applyDmgOverTime;
            return this;
        }

        /// <summary>
        /// Sets the duration for damage over time.
        /// </summary>
        public AreaDamageContext SetDamageDuration(float damageDuration)
        {
            DamageDuration = damageDuration;
            return this;
        }

        /// <summary>
        /// Sets the tick interval for damage over time.
        /// </summary>
        public AreaDamageContext SetTickInterval(float tickInterval)
        {
            TickInterval = tickInterval;
            return this;
        }

        /// <summary>
        /// Sets the object pool used for creating AreaDamageInstance.
        /// </summary>
        public AreaDamageContext SetPool(IObjectPool<AreaDamageInstance> pool)
        {
            Pool = pool;
            return this;
        }

        public AreaDamageContext SetAttacker(EntityBase attacker)
        {
            this.Attacker = attacker;
            return this;
        }

        /// <summary>
        /// Resets the context to default values.
        /// </summary>
        public AreaDamageContext Reset()
        {
            DMG = 0f;
            Radius = 0f;
            TargetLayer = default;
            ApplyDmgOverTime = false;
            DamageDuration = 0f;
            TickInterval = 0f;
            Pool = null;
            Attacker = null;
            return this;
        }
    }

        

    public class AreaDamageInstance : EntityBase
    {

        private AreaDamageContext currentContext;
        private float damageRadius => currentContext.Radius;// The radius within which damage is applied
        private float damageAmount => currentContext.DMG; // The amount of damage dealt
        private LayerMask targetLayer => currentContext.TargetLayer; // Defines which objects are affected by the damage

        private bool applyDamageOverTime => currentContext.ApplyDmgOverTime; // Enable damage over time

        private float damageDuration => currentContext.DamageDuration; // Duration for damage over time

        private float tickInterval => currentContext.TickInterval; // Time interval between damage ticks

        //[Header("Visual Effects")]
        //[SerializeField] private GameObject impactEffect; // Effect to instantiate when damage is applied


        public Action OnFinishDealDmg;

        public void Init(AreaDamageContext context)
        {
            currentContext = context;   

            transform.localScale = Vector3.one * damageRadius;
        }

        /// <summary>
        /// Triggers the area damage logic.
        /// </summary>
        public void TriggerAreaDamage()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, targetLayer);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<IDamageable>(out IDamageable target))
                {
                    if (applyDamageOverTime)
                    {
                        StartCoroutine(ApplyDamageOverTime(target));
                    }
                    else
                    {
                        target.TakeDamage( this, damageAmount);
                    }
                }

                // Trigger visual effects if any
                //if (impactEffect != null)
                //{
                //    Instantiate(impactEffect, hitCollider.transform.position, Quaternion.identity);
                //}
            }

            StartCoroutine(DelayRelease());
        }

        private IEnumerator ApplyDamageOverTime(IDamageable hitTagret)
        {
            float elapsed = 0f;

            while (elapsed <= damageDuration)
            {
                hitTagret.TakeDamage( currentContext.Attacker , damageAmount * (tickInterval / damageDuration));
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }

            
        }

        private IEnumerator DelayRelease()
        {
            yield return new WaitForSeconds(damageDuration);
            currentContext.Pool.Release(this);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize the damage radius in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }
    }
}
