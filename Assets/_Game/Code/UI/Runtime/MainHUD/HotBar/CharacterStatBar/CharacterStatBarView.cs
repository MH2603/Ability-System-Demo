using MH.UISystem;
using UnityEngine;

namespace MH.UI
{
    public class CharacterStatBarViewModel : UIViewModel
    {
        public StatBarViewModel HpBarViewModel { get; set; } = new();
        public StatBarViewModel ManaBarViewModel { get; set; } = new();
    }
    
    public class CharacterStatBarView : UIView<CharacterStatBarViewModel>
    {
        [SerializeField] private StatBarView _hpBar;
        [SerializeField] private StatBarView _manaBar;
        
        protected override void OnSetViewModel(CharacterStatBarViewModel vBarViewModel)
        {
            _hpBar.SetViewModel(vBarViewModel.HpBarViewModel);
            _manaBar.SetViewModel(vBarViewModel.ManaBarViewModel);
        }
    }
}