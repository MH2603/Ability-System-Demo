using UnityEngine;

namespace MH.Ability
{
    // Enum for different types of ability activations
    public enum EAbilityActivationType
    {
        Instant,           // Cast immediately on press (like Flash)
        TargetPoint,       // Requires clicking a point on ground (like Meteor)
        TargetDirection,   // Requires a direction (like Skillshot)
        TargetEnemy,       // Requires clicking an enemy (like targeted spells)
        SelfCast          // Cast on self (like buffs)
    }

    // ScriptableObject to configure ability input settings
    [CreateAssetMenu(fileName = "AbilityInputConfig", menuName = "MH/Ability/InputConfig")]
    public class AbilityInputConfigSO : ScriptableObject
    {
        public string abilityName;
        public KeyCode primaryKey;
        public EAbilityActivationType activationType;
        public float maxTargetingRange = 10f;
        public LayerMask targetLayers;
        public GameObject targetingIndicatorPrefab;
    }

    [System.Serializable]
    public class AbilityInputConfig
    {
        public KeyCode primaryKey;
        public EAbilityActivationType activationType;
        public LayerMask DetectLayer;
        public GameObject targetingIndicatorPrefab;
    }
}
