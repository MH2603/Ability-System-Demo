using MH.UISystem;
using TMPro;
using UniRx;
using UnityEngine;  
using UnityEngine.UI;  

namespace MH.UI
{
    public class StatBarViewModel : UIViewModel
    {
        public ReactiveProperty<float> MaxValue = new ();    
        public ReactiveProperty<float> CurrentValue = new ();
    }
    
    public class StatBarView : UIView<StatBarViewModel>
    {
        [SerializeField] private Image _fill;
        [SerializeField] private TextMeshProUGUI _currentOverMaxText;

        private float _maxValue => ViewModel.MaxValue.Value;
        private float _currentValue => ViewModel.CurrentValue.Value;
        
        protected override void OnSetViewModel(StatBarViewModel vModel)
        {
            viewModel.CurrentValue.Subscribe(value => UpdateViusal()).AddTo(compositeDisposable);
            viewModel.MaxValue.Subscribe(value => UpdateViusal()).AddTo(compositeDisposable);
        }

        private void UpdateViusal()
        {
            _fill.fillAmount = _currentValue / _maxValue;
            _currentOverMaxText.text = $"{_currentValue:F0} / {_maxValue:F0}";
        }
            
    }
}