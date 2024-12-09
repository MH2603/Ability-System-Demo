using Foundation.EntitySystem;
using Foundation.Extensions;
using MH.Character;
using System;
using UnityEngine;

namespace MH.Projectile
{
    [Serializable]
    public struct ProjectileContext
    {
        // common
        public float Speed { get; private set; }
        public LayerMask CanHitLayer { get; private set; }

        // forward projectile
        public Vector3 Direction { get; private set; }

        // target projectile
        public float RotationSpeed { get; private set; }
        public Transform Target { get; private set; }

        public void ResetData()
        {
            Speed = 0;
            CanHitLayer = 0;
            Direction = Vector3.zero;
            RotationSpeed = 0;  
            Target = null;
        }

        public ProjectileContext SetSpeed(float speed) { this.Speed = speed; return this; }
        public ProjectileContext SetCanHitLayer(LayerMask canHitLayer) { this.CanHitLayer = canHitLayer; return this; }
        public ProjectileContext SetDirection(Vector3 direction) { this.Direction = direction; return this; }
        public ProjectileContext SetRotationSpeed(float rotationSpeed) { this.RotationSpeed = rotationSpeed; return this; }
        public ProjectileContext SetTarget(Transform target) { this.Target = target; return this; }
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class BaseProjectile : EntityBase
    {
        protected Rigidbody rb;

        protected ProjectileContext data;   
        protected Action<BaseProjectile,GameObject> hitAction;

        #region ----------------- Unity Methods ----------------

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (CanHit(collider))
            {
                OnHit(collider.gameObject);
            }
        }

        #endregion


        #region --------------- Public Methods -----------------

        public virtual void ResetRuntimeProperties()
        {

        }


        public void TransitionData(ProjectileContext data)
        {
            this.data = data;
        }

        public void RegisterOnHitEvent(Action<BaseProjectile, GameObject> callback)
        {
            hitAction += callback;
        }

        public void UnRegisterOnHitEvent(Action<BaseProjectile, GameObject> callback)
        {
            hitAction -= callback;
        }

        public virtual void Shoot()
        {
            if (!rb) rb = GetComponent<Rigidbody>();

            rb.velocity = data.Direction.normalized * data.Speed;
        }

        #endregion


        protected virtual void OnHit(GameObject hitEntity)
        {
            hitAction?.Invoke( this,hitEntity);
        }

        protected virtual bool CanHit(Collider collider)
        {
            if ( collider.TryGetComponent<IDamageable>(out IDamageable hitEntity) &&
                 IsLayerCanHit(collider.gameObject) )
            {
                return true;
            }
            
            return false;
        }

        protected virtual bool CanHit(GameObject gameObject)
        {
            return true;
        }

        protected virtual bool IsLayerCanHit(GameObject entity)
        {
            return LayerMaskExtensions.Contains(data.CanHitLayer, entity.layer);
        }

        
    }

}
