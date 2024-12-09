using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MH.UISystem
{
    /// <summary>
    /// the base class for all UI Layer in UI game
    /// </summary>
    public abstract class UILayer : MonoBehaviour
    {
        /// <summary>
        /// Sets the active state of the UI Layer.
        /// </summary>
        /// <param name="active"></param>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous operation.</returns>
        public virtual UniTask SetActiveAsync(bool active)
        {
            gameObject.SetActive(active);
            return OnSetActive(active);
        }
        
        
        /// <summary>
        /// Override this method to perform additional operations when the active state of the UI layer is set.
        /// Use UniTask to async action. Ex: await PlayFadeInAnimation() -> return complete
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        protected virtual UniTask OnSetActive(bool active)
        {
            return UniTask.CompletedTask;
        }
    }

    
    /// <summary>
    /// The base class for all UI layers in game
    /// We are using CRTP to allow for easy access to main instance of the layer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UILayer<T> : UILayer where T : UILayer<T>
    {
        public static T Main
        {
            get
            {
                T main = UIManager.Instance.GetMain<T>();
                if (main) return main;
                return null;
            }
        }
    }
}