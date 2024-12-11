using Foundation.EntitySystem;
using Foundation.Extensions;
using MH.Ability;
using MH.Projectile;
using MH.Skill.Archer;
using UnityEngine;
using UnityEngine.Pool;

namespace MH.Character
{
    public enum ENormalAttackState
    {
        None = 0,
        Chasing,
        Attacking,
        Wating
    }

    public class CharacrerNormalAttackState : CharacterBaseState
    {
        #region ------------ Properties ---------------------

        private StatsMediator _statsMediator;    
        private RangeNormalAttackSO _config;

        private Transform root;
        private Transform firePoint;

        private IObjectPool<BaseProjectile> _projectilePool;

        private Transform _currentTarget;
        private ENormalAttackState _state;
        private ProjectileContext _projectileData;
        private float _lastAttackTime;

        private float distance => Vector3.Distance(root.position, _currentTarget.position);
        private bool canAttack => distance <= attackRadius;
        private float dmg => _statsMediator.GetFinalStatValue(AbilityStatTypeString.Convert(EAbilityStat.Dmg));
        private float attackRadius => GetFinalAbilityStat(EAbilityStat.Dmg);
        private float projectileSpeed => GetFinalAbilityStat(EAbilityStat.ProjectileSpeed);
        private float projectileRotateSpeed => GetFinalAbilityStat(EAbilityStat.ProjectileRotateSpeed);
        private float attackSpeed => characterStatController.GetFinalStatValue("AttackSpeed");

        #endregion

        #region ------------- Base State Methods ------------

        public CharacrerNormalAttackState(EntityBase entity, CharacterStateMachine stateMachine) : base(entity, stateMachine)
        {
            
        }

        public override void OnStateStart()
        {
            base.OnStateStart();

            SetUp();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            StartChasing();
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            if( _state == ENormalAttackState.Chasing)
            {
                MoveToTarget();
            }

            if ( _state == ENormalAttackState.Wating)
            {
                Wating();
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            _currentTarget = null;
        }

        #endregion


        private void SetUp()
        {
            root = entity.transform;
            firePoint = PointContainer.GetPoint(PointNames.FirePoint).Point;
            _config = CharacterAbility.GetAbilityConfig("Normal Attack") as RangeNormalAttackSO;

            _statsMediator = new StatsMediator();
            foreach (var baseStat in _config.Stats)
            {
                _statsMediator.TryAddStat(baseStat);
            }

            _projectilePool = new ObjectPool<BaseProjectile>(
                                createFunc: () => CreateNewProjectile(), // Create new instances
                                actionOnGet: null, // Action when taking from pool
                                actionOnRelease: obj => RelaeseProjectile(obj), // Action when returning to pool
                                actionOnDestroy: null, // Action when pool capacity is exceeded
                                collectionCheck: true, // Check for duplicates when returning to pool
                                defaultCapacity: 10, // Initial capacity
                                maxSize: 20 // Maximum pool size
                            );

            MouseRaycaster.OnRightClick += OnRightClick;
            AnimationEvent.RegisterEvent("Shoot", Shoot);

            _projectileData = new ProjectileContext();
        }

        protected float GetFinalAbilityStat(EAbilityStat statType)
        {
            return _statsMediator.GetFinalStatValue(AbilityStatTypeString.Convert(statType));
        }
        private BaseProjectile CreateNewProjectile()
        {
            var newProjectile = _config.CreateNewProjectile();
            newProjectile.gameObject.SetActive(false);
            newProjectile.RegisterOnHitEvent(OnProjectileHit);

            return newProjectile;
        }

        private void RelaeseProjectile(BaseProjectile projectile)
        {
            projectile.ResetRuntimeProperties();
            projectile.gameObject.SetActive(false);
        }
        

        private void OnRightClick(ClickEventData data)
        {
            Debug.Log($" LOG : Right click - Layer: {data.GameObject.layer} !");
            if ( LayerMaskExtensions.Contains(_config.EnemyLayer, data.GameObject.layer) )
            {
                Debug.Log(" LOG : Click on enemy !");
                _currentTarget = data.GameObject.transform;
                StateMachine.TransitionTo(ECharacterState.Attack);
            }
            else if(_currentTarget != null)
            {
                StateMachine.TransitionTo(ECharacterState.Locomotion);
            }
        }

        private void StartChasing()
        {
            _state = ENormalAttackState.Chasing;
        }

        private void MoveToTarget()
        {
            if( canAttack )
            {
                StartAttack();
                return;
            }

            Vector3 direction = _currentTarget.position - root.position;
            Vector2 moveDir = Vector2.zero;
            moveDir.x = direction.x;
            moveDir.y = direction.z;

            CharacterMovement.Move(moveDir);
        }

        private void StartAttack()
        {
            _state = ENormalAttackState.Attacking;

            CharacterAnimationController.PlayAnimation(AnimationNames.NormalAttack);

            CharacterMovement.RotateToTarget(_currentTarget.position);
            CharacterMovement.Move(Vector2.zero);

            _lastAttackTime = Time.time;

            StartWaiting();
        }

        private void Shoot()
        {
            _projectileData = _projectileData.SetSpeed(projectileSpeed).
                                              SetRotationSpeed(projectileRotateSpeed).
                                              SetCanHitLayer(_config.EnemyLayer).
                                              SetTarget(_currentTarget).
                                              SetDirection(root.forward);

            BaseProjectile projectile = _projectilePool.Get();
            projectile.transform.position = firePoint.position;
            projectile.gameObject.SetActive(true);
            projectile.TransitionData(_projectileData);

            projectile.Shoot();
        }

        private void StartWaiting()
        {
            _state = ENormalAttackState.Wating;
        }

        private void Wating()
        {
            if ( Time.time - _lastAttackTime >= 1/attackSpeed)
            {
                if (canAttack) StartAttack();
                else StartChasing();
            }
        }

        private void OnProjectileHit(BaseProjectile hitProjectile ,GameObject hitter)
        {
            if ( hitter.transform == _currentTarget )
            {
                _projectilePool.Release(hitProjectile);
                hitter.GetComponent<IDamageable>().TakeDamage(entity ,dmg);
            }
        }


    }

}
