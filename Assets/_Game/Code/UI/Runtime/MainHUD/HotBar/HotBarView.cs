using System.Collections.Generic;
using MH.UI.Ability.RuntimeStats;
using MH.UISystem;
using UnityEngine;

namespace MH.UI.Ability
{
    public class HotBarViewModel : UIViewModel
    {
        // abilities
        public List<HotBarAbilityViewModel> Abilities = new();

        // character stats
        public List<RuntimeStatViewModel> Stats = new();
    }
    
    public class HotBarView : UIView<HotBarViewModel>
    {
        #region  -------- Inspectors --------------

        [Header(" Abilities")] 
        [SerializeField] private HotBarAbilityView abilityViewPrefab;
        [SerializeField] private Transform abilityViewContainer;
        
        [Header(" Runtime Stats ")]
        [SerializeField] private RuntimeStatView statsViewPrefab;
        [SerializeField] private Transform statsViewContainer;
        
        #endregion
        
        #region  -------- Properties --------------
        
        private Dictionary<string, HotBarAbilityView> abilityViewMap = new();
        
        #endregion
        
        protected override void OnSetViewModel(HotBarViewModel vModel)
        {
            // Debug.Log("Step 03!");
            foreach (var abilityViewModel in vModel.Abilities)
            {
                var newAbilityView = Instantiate(abilityViewPrefab, abilityViewContainer);
                newAbilityView.SetViewModel(abilityViewModel);
                abilityViewMap[abilityViewModel.AbilityName.Value] = newAbilityView;    
            }
            
            foreach (var statViewModel in vModel.Stats)
            {
                var newStatView = Instantiate(statsViewPrefab, statsViewContainer);
                newStatView.SetViewModel(statViewModel);
                
            }
        }
    }
}