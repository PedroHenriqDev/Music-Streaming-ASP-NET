using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class ListenerFactoriesFacade
    {
        private readonly ViewModelFactory _viewModelFactory;

        public ListenerFactoriesFacade(ViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public ListenerPageViewModel FacListenerPageVM(Listener listener)
        {
            return _viewModelFactory.FacListenerPageVM(listener);
        }

        public async Task<DescriptionViewModel> FacListenerDescriptionVMAsync(Listener listener)
        {
            return await _viewModelFactory.FacListenerDescriptionVMAsync(listener);
        }
    }
}
