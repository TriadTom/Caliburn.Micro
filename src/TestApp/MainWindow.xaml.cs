using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using SimpleInjector;
using TestApp.Messages;
using TestApp.ViewModels;
using SimpleInjector;
using TestApp.UI;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IHandle<EventAggregator.MessageAdded>, IHandle<EventAggregator.MessageRemoved> {
        readonly List<object> _objectList = new List<object>();
        public MainWindow()
        {
            InitializeComponent();
            App.IoCContainer = new Container();
            App.IoCContainer.Register<IEventAggregator, EventAggregator>(Lifestyle.Singleton);
            App.EventAgrigator = (IEventAggregator)App.IoCContainer.GetInstance(typeof(IEventAggregator));
        }
        
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            App.EventAgrigator.Subscribe(this);
        }

        private void Garbage_OnClick(object sender, RoutedEventArgs e) {
            GC.Collect();
        }


        private void Display_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var f in App.EventAgrigator.ActiveFiltersForType(typeof(Message1))) {
                Debug.WriteLine("Filter for Message1: " + f);
            }
            
        }
        private void Display_OnClick2(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Count for Message1: " + App.EventAgrigator.HandlerCountFor(typeof(Message1)));
            Debug.WriteLine("Count for Message2: " + App.EventAgrigator.HandlerCountFor(typeof(Message2)));
            Debug.WriteLine("Count for Message3: " + App.EventAgrigator.HandlerCountFor(typeof(Message3)));
            Debug.WriteLine("Count for Message4: " + App.EventAgrigator.HandlerCountFor(typeof(Message4)));
            Debug.WriteLine("Count for Message5: " + App.EventAgrigator.HandlerCountFor(typeof(Message5)));
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb) {
                case 1:
                    ob = _objectList.OfType<ViewModel1>().FirstOrDefault();
                    break;
                case 2:
                    ob = _objectList.OfType<ViewModel2>().FirstOrDefault();
                    break;
                case 3:
                    ob = _objectList.OfType<ViewModel3>().FirstOrDefault();
                    break;
                case 4:
                    ob = _objectList.OfType<ViewModel4>().FirstOrDefault();
                    break;
                case 5:
                    ob = _objectList.OfType<ViewModel5>().FirstOrDefault();
                    break;
            }
            if (ob != null)
                _objectList.Remove(ob);
        }

        private void Message_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            App.EventAgrigator.PublishOnCurrentThread(new Message1(), "M" + Regex.Replace(btn.Name, "[^0-9.]", ""));
            //App.EventAgrigator.PublishOnCurrentThread(new Message1(), "M1");
        }
        private void Message_OnClick2(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb)
            {
                case 1:
                    ob = new Message1();
                    break;
                case 2:
                    ob = new Message2();
                    break;
                case 3:
                    ob = new Message3();
                    break;
                case 4:
                    ob = new Message4();
                    break;
                case 5:
                    ob = new Message5();
                    break;
            }
            if (ob != null)
                App.EventAgrigator.PublishOnCurrentThread(ob, "M1");
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb)
            {
                case 1:
                    ob = new ViewModel1(App.EventAgrigator);
                    break;
                case 2:
                    ob = new ViewModel2(App.EventAgrigator);
                    break;
                case 3:
                    ob = new ViewModel3(App.EventAgrigator);
                    break;
                case 4:
                    ob = new ViewModel4(App.EventAgrigator);
                    break;
                case 5:
                    ob = new ViewModel5(App.EventAgrigator);
                    break;
            }
            if (ob != null)
                _objectList.Add(ob);
        }

        public void Handle(EventAggregator.MessageAdded message) {
            Debug.WriteLine("MainWindow MessageAdded: "+message.Type.Name+"-"+message.Filter+ " Remaining: " + App.EventAgrigator.HandlerCountFor(message.Type, message.Filter));
        }

        public void Handle(EventAggregator.MessageRemoved message)
        {
            Debug.WriteLine("MainWindow MessageRemoved: " + message.Type.Name + "-" + message.Filter + " Remaining: " + App.EventAgrigator.HandlerCountFor(message.Type, message.Filter));
        }
    }
}
