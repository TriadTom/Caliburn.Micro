using System;
using Caliburn.Micro;

namespace TestApp.ViewModels
{
    public class ViewModelBase
    {
        private IEventAggregator _eventAggrigator;
        public ViewModelBase(IEventAggregator eventAggrigator) {
            //Debug.WriteLine(GetType() + " Created");
            _eventAggrigator = eventAggrigator;
            _eventAggrigator.Subscribe(this);
        }

        ~ViewModelBase() {
            //Debug.WriteLine("Destructor Callled for " + this.GetType());
            _eventAggrigator.Unsubscribe(this);

        }
    }
}
