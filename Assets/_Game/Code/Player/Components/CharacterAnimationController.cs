using Foundation.EntitySystem;
using UnityEngine;

namespace MH.Character
{
    public enum EAnimationState
    {
        None,
        Locomotion
    }


    public static class AnimationNames
    {
        public const string Locomotion = "Locomotion";
        public const string NormalAttack = "NormalAttack";
    }

    public static class AnimationParematers
    {
        public const string Speed = "Speed";
    }


    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationController : EntityComponent
    {
        private Animator animator;

        public override void ManualStart()
        {
            base.ManualStart();

            animator = GetComponent<Animator>();
        }

        public void PlayAnimation(string animationName, float trasitionDuration=0.1f)
        {
            //animator.Play(animationName);
            animator.CrossFade(animationName, trasitionDuration);
        }

        public void SetFloatParamater(string paramaterName, float value)
        {
            animator.SetFloat(paramaterName, value);
        }
    }
}
