using MH.UISystem;
using UnityEngine;

namespace MH.UI
{
    public class CharacterStatModelView : UIViewModel
    {
        public StatBarViewModel HpBarViewModel { get; set; } = new();
        public StatBarViewModel ManaBarViewModel { get; set; } = new();
    }
    
    public class CharacterStatView : UIView<CharacterStatModelView>
    {
        [SerializeField] private StatBarView _hpBar;
        [SerializeField] private StatBarView _manaBar;
        
        protected override void OnSetViewModel(CharacterStatModelView vModel)
        {
            _hpBar.SetViewModel(vModel.HpBarViewModel);
            _manaBar.SetViewModel(vModel.ManaBarViewModel);
        }
    }
}