using MH.UI.Ability;
using MH.UISystem;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH.UI
{
    public class MainHUDViewModel : UIViewModel
    {
        public CharacterStatBarViewModel CharacterStatBarViewModel = new();
        public HotBarViewModel HotBarViewModel = new();
    }
    
    public class MainHUDView  : UIView<MainHUDViewModel>
    {
        [SerializeField] private CharacterStatBarView characterStatBarView;
        [SerializeField] private HotBarView hotBarView; 
        protected override void OnSetViewModel(MainHUDViewModel vModel)
        {
            characterStatBarView.SetViewModel(vModel.CharacterStatBarViewModel);
            // Debug.Log("Step 02!");
            hotBarView.SetViewModel(vModel.HotBarViewModel);    
        }
    }
}