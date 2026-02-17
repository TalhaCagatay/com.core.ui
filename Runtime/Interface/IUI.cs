using System;
using Cysharp.Threading.Tasks;

namespace core.com.ui
{
    public interface IUI
    {
        event Action<IUI> Showed;
        event Action<IUI> Hided;

        UniTask ShowAsync();
        UniTask HideAsync();
    }
}