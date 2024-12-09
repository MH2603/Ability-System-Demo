using MH.UISystem;
using TMPro;
using UniRx;
using UnityEngine;

namespace MH.UI.Ability.RuntimeStats
{
    public class RuntimeStatViewModel : UIViewModel
    {
        public ReactiveProperty<string> StatName = new();
        public ReactiveProperty<float> StatValue = new();
    }
        
    
    public class RuntimeStatView : UIView<RuntimeStatViewModel>
    {
        #region  ----- Inspector ------------

        [SerializeField] private TextMeshProUGUI statText;  

        #endregion
        
        protected override void OnSetViewModel(RuntimeStatViewModel vModel)
        {
            vModel.StatName.Subscribe(name => UpdateStatText()).AddTo(compositeDisposable);
            vModel.StatValue.Subscribe(value => UpdateStatText()).AddTo(compositeDisposable);
        }

        private void UpdateStatText()
        {
            statText.text = $"{ViewModel.StatName.Value} : {ViewModel.StatValue.Value:F0}";
        }
    }
}