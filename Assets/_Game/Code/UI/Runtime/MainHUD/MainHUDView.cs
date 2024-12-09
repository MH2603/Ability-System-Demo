using MH.UISystem;
using UniRx;
using UnityEngine;

namespace MH.UI
{
    public class MainHUDViewModel : UIViewModel
    {
        public CharacterStatModelView characterStatModelView = new();

    }
    
    public class MainHUDView  : UIView<MainHUDViewModel>
    {
        [SerializeField] private CharacterStatView _characterStatView;
        
        protected override void OnSetViewModel(MainHUDViewModel vModel)
        {
            _characterStatView.SetViewModel(vModel.characterStatModelView);
        }
    }
}