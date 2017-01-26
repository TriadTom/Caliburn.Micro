using System.Windows;
using Caliburn.Micro;
using SimpleInjector;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IEventAggregator EventAgrigator;
        public static Container IoCContainer;


    }
}
