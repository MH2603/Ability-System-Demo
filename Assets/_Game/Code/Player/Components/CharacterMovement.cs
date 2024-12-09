using Foundation.EntitySystem;
using UnityEngine;

namespace MH.Character
{

    public class CharacterMovement : EntityComponent
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _hitPushBackSpeed = 1f;
        [SerializeField] private float _dashSpeed = 15f;
        [SerializeField] private float _rotationSmooth = .1f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _gravity = 10f;


        private Vector2 _moveDirection;
        private float _velocity;
        private Vector3 _dashDirection;
        private bool _isDashing;
        private bool _isIgnoreGravity;
        private Vector3 _hitDirection;
        //private CharacterAnimationController CharacterAnimationController => entity.Get<CharacterAnimationController>();

        public bool IsGrounded => Physics.Raycast(_groundCheck.position, Vector3.down, 0.1f, _groundMask);

        public override void ManualUpdate()
        {
            base.ManualUpdate();
            UpdateRotation();
            UpdateGravity();
        }

        private void UpdateGravity()
        {
            if (!IsGrounded && !_isIgnoreGravity)
            {
                _characterController.Move(Vector3.down * (_gravity * Time.deltaTime));
            }
        }

        private void UpdateRotation()
        {
            if (_moveDirection != Vector2.zero)
            {
                var targetRotation = Mathf.Atan2(_moveDirection.x, _moveDirection.y) * Mathf.Rad2Deg;
                var rotation =
                    Mathf.SmoothDampAngle(entity.transform.eulerAngles.y, targetRotation, ref _velocity,
                                          _rotationSmooth);
                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }
        }

        public void Move(Vector2 direction)
        {
            _moveDirection = direction.normalized;
            var motionDir = new Vector3(_moveDirection.x, 0, _moveDirection.y);
            //CharacterAnimationController.MovementSpeed = _moveDirection.magnitude;
            _characterController.Move(motionDir * (_speed * Time.deltaTime));
        }

        public void RotateToTarget(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            var targetRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        }

        public void Dash(Vector2 direction)
        {
            _dashDirection = new Vector3(direction.x, 0, direction.y);
        }

        public void Jump(bool start = true)
        {
            _isIgnoreGravity = start;
        }

        public void UpdateDash()
        {
            _characterController.Move(_dashDirection * (_dashSpeed * Time.deltaTime));
        }

        public void UpdateJump()
        {
            _characterController.Move(Vector3.up * (_jumpForce * Time.deltaTime));
        }

        public void PushBack(Vector3 hitDirection)
        {
            _hitDirection = hitDirection;
        }

        public void UpdatePushBack()
        {
            _characterController.Move(_hitDirection * (_hitPushBackSpeed * Time.deltaTime));
        }
    }

}
