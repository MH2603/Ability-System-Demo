using UnityEngine;

namespace MH.Projectile.TargetBullet
{
    public class TargetProjectile : BaseProjectile 
    {

        private Transform root;


        protected override void Awake()
        {
            base.Awake();

            root = GetComponent<Transform>();   
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            RotateToTarget(data.Target.position, data.RotationSpeed);
            MoveFoward();
        }


        public override void Shoot()
        {
            base.Shoot();
        }

        protected void RotateToTarget(Vector3 positionTarget, float rotationSpeed)
        {
            Vector3 direction = positionTarget - transform.position;

            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }

        private void MoveFoward()
        {
            rb.velocity = root.forward.normalized * data.Speed;
        }

        

    }
}
