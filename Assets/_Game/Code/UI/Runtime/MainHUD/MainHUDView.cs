using MH.UISystem;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH.UI
{
    public class MainHUDViewModel : UIViewModel
    {
        public CharacterStatBarViewModel CharacterStatBarViewModel = new();

    }
    
    public class MainHUDView  : UIView<MainHUDViewModel>
    {
        [FormerlySerializedAs("_characterStatView")] [SerializeField] private CharacterStatBarView characterStatBarView;
        
        protected override void OnSetViewModel(MainHUDViewModel vModel)
        {
            characterStatBarView.SetViewModel(vModel.CharacterStatBarViewModel);
        }
    }
}