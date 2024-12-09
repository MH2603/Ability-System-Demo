using Foundation.EntitySystem;
using MH.Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MH.Ability
{

    // Component to handle ability inputs and targeting
    public class AbilityInputHandler : EntityComponent
    {

        #region ---------------- Inspector ----------------

        #endregion

        #region ---------------- Properties ----------------

        private CharacterAbilityManager abilityManager;

        private Dictionary<string, AbilityInputConfig> _abilityInputConfigMap = new();
        private Dictionary<string, IAbilityIndicator> _indicatorsMap = new();

        private Transform body;
        private Camera mainCamera;

        private IAbilityIndicator currentIndicator; // indicator is targeting
        private string currentAbilityName = ""; // name of ability indicator is targeting
        private AbilityInputConfig currentInputConfig => _abilityInputConfigMap.ContainsKey(currentAbilityName) ?
                                                         _abilityInputConfigMap[currentAbilityName] : null;    

        private Vector3 myPos => body.position;
        private float attackLimitMax => abilityManager.GetAbility(currentAbilityName).AttackLimitMax;
        private float dmgRadius => abilityManager.GetAbility(currentAbilityName).DmgRadius;

        #endregion

        #region ------------- Entity Methods -----------------

        public override void ManualStart()
        {
            base.ManualStart();

            abilityManager = entity.Get<CharacterAbilityManager>();
            mainCamera = Camera.main;
            body = transform;

            foreach (var abilityConfig in abilityManager.GetAllAbilityConfig())
            {
                _abilityInputConfigMap[abilityConfig.AbilityName] = abilityConfig.InputConfig;

                GameObject newIndicator = Instantiate(abilityConfig.InputConfig.targetingIndicatorPrefab);
                newIndicator.SetActive(false);

                _indicatorsMap[abilityConfig.AbilityName] = newIndicator.GetComponent<IAbilityIndicator>();
            }

           
        }

        public override void ManualUpdate()
        {
            // Check for ability activation keys
            foreach (var abilityName in _abilityInputConfigMap.Keys.ToList())
            {
                if (Input.GetKeyDown(_abilityInputConfigMap[abilityName].primaryKey))
                {
                    HandleAbilityActivation(abilityName);
                }
            }

            // Handle targeting if active
            if (currentInputConfig != null)
            {
                UpdateTargeting();
            }
        }

        #endregion

        #region -------------- Private Methods --------------------

        #region ----- Controller Ability Input ---

        private void HandleAbilityActivation(string abilityName)
        {
            // Check if ability is ready (cooldown, resources, etc.)
            
            if (!abilityManager.IsAbilityReady(abilityName))
            {
                // Maybe show cooldown feedback
                return;
            }

            AbilityInputConfig config = _abilityInputConfigMap[abilityName];
            switch (config.activationType)
            {
                case EAbilityActivationType.Instant:
                    ExecuteInstantAbility(abilityName);
                    break;

                case EAbilityActivationType.SelfCast:
                    ExecuteSelfCastAbility(abilityName);
                    break;

                case EAbilityActivationType.TargetPoint:
                case EAbilityActivationType.TargetDirection:
                case EAbilityActivationType.TargetEnemy:
                    StartTargeting(abilityName);
                    break;
            }
        }

        private void StartTargeting(string abilityName)
        {
            // Cancel any existing targeting
            CancelTargeting();

            // Start new targeting
            currentAbilityName = abilityName;
            currentIndicator = _indicatorsMap[abilityName];
            currentIndicator.Init(body, attackLimitMax, dmgRadius);
        }

        private void UpdateTargeting()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, currentInputConfig.DetectLayer))
            {
                // Update targeting indicator position
                if (currentInputConfig != null)
                {
                    Vector3 targetPos = hit.point;
                    // Clamp to max range if needed
                    if (Vector3.Distance(myPos, targetPos) > attackLimitMax)
                    {
                        targetPos = transform.position +
                            (targetPos - transform.position).normalized * attackLimitMax;
                    }

                    //currentTargetingIndicator.transform.position = targetPos;
                    currentIndicator.SetTargetPos(targetPos);
                }

                // Handle click to execute
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPos = currentIndicator.GetTargetPos();
                    switch (currentInputConfig.activationType)
                    {
                        case EAbilityActivationType.TargetPoint:
                            ExecuteTargetPointAbility(targetPos, currentAbilityName);
                            break;

                        case EAbilityActivationType.TargetDirection:
                            Vector3 direction = (hit.point - transform.position).normalized;
                            ExecuteDirectionalAbility(currentInputConfig, direction, currentAbilityName);
                            break;

                        case EAbilityActivationType.TargetEnemy:
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                            {
                                ExecuteTargetEnemyAbility( hit.transform, currentAbilityName);
                            }
                            break;
                    }
                }
            }

            // Handle targeting cancellation
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelTargeting();
            }
        }

        private void CancelTargeting()
        {
            if (currentIndicator == null) return;

            currentAbilityName = "";
            currentIndicator.Off();
            currentIndicator = null;
        }

        #endregion

        #region ---- Kind of execute ------

        private void ExecuteInstantAbility(string abilityName)
        {
            var context = new AbilityContext
            {
                Direction = transform.forward,
                TargetPosition = transform.position
            };

            abilityManager.CastAbility(abilityName, context);
        }

        private void ExecuteSelfCastAbility(string abilityName)
        {
            var context = new AbilityContext
            {
                Target = transform,
                TargetPosition = transform.position
            };

            abilityManager.CastAbility(abilityName, context);
        }

        private void ExecuteTargetPointAbility(Vector3 targetPoint, string abilityName)
        {
            

            var context = new AbilityContext
            {
                TargetPosition = targetPoint,
                Direction = (targetPoint - transform.position).normalized
            };

            abilityManager.CastAbility(abilityName, context);
            CancelTargeting();
        }

        private void ExecuteDirectionalAbility(AbilityInputConfig config, Vector3 direction, string abilityName)
        {
            var context = new AbilityContext
            {
                Direction = direction,
                TargetPosition = transform.position + direction * attackLimitMax
            };

            abilityManager.CastAbility(abilityName, context);
            CancelTargeting();
        }

        private void ExecuteTargetEnemyAbility( Transform target, string abilityName)
        {
            
            if (Vector3.Distance(transform.position, target.position) > attackLimitMax) 
            {
                CancelTargeting();
                return; 
            }

            var context = new AbilityContext
            {
                Target = target,
                TargetPosition = target.position,
                Direction = (target.position - transform.position).normalized
            };

            abilityManager.CastAbility(abilityName, context);
            CancelTargeting();
        }

        #endregion

        #endregion
    }
}
