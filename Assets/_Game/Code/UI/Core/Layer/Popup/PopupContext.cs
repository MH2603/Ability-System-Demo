namespace MH.UISystem.Popup
{
    public interface IPopupContext
    {
        
    }
    
    public abstract class PopupContext<TModel, TView, TViewModel> : BaseUIContext<TModel, TView, TViewModel>,
        IPopupContext
        where TModel : UIModel
        where TView : UIView<TViewModel>
        where TViewModel : UIViewModel
    {
    }
}