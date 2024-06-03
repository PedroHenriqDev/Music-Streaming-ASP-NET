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

        public async Task<ListenerPageViewModel> FacListenerPageVMAsync(Listener listener)
        {
            return await _viewModelFactory.FacListenerPageVMAsync(listener);
        }

        public async Task<DescriptionViewModel> FacListenerDescriptionVMAsync(Listener listener)
        {
            return await _viewModelFactory.FacListenerDescriptionVMAsync(listener);
        }
    }
}
