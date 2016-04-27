﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XTMF.Gui.UserControls
{
    /// <summary>
    /// Interaction logic for FreeVariableEntry.xaml
    /// </summary>
    public partial class FreeVariableEntry : Window
    {
        private readonly Type[] Conditions;
        private readonly ModelSystemEditingSession Session;
        public FreeVariableEntry(Type freeVariable, ModelSystemEditingSession session)
        {
            InitializeComponent();
            Session = session;
            FilterBox.Filter = CheckAgainstFilter;
            FilterBox.Display = Display;
            Conditions = freeVariable.GetGenericParameterConstraints();
            Loaded += FreeVariableEntry_Loaded;
        }

        class Model : INotifyPropertyChanged
        {
            internal Type type;

            public Model(Type type)
            {
                this.type = type;
            }

            public string Name { get { return type.Name; } }

            public string Text { get { return type.FullName; } }

            public event PropertyChangedEventHandler PropertyChanged;

            internal static List<Model> CreateModel(ICollection<Type> types)
            {
                return types.Select(t => new Model(t)).OrderBy(t => t.Name).ToList();
            }
        }

        private void FreeVariableEntry_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
           {
               var temp = Model.CreateModel(Session.GetValidGenericVariableTypes(Conditions));
               Dispatcher.Invoke(() =>
              {
                  Display.ItemsSource = (AvailableModules = temp);
                  
              });
           });
        }

        public Type SelectedType { get; private set; }

        private List<Model> AvailableModules;

        private bool CheckAgainstFilter(object o, string text)
        {
            var model = o as Model;
            if (string.IsNullOrWhiteSpace(text)) return true;
            if (model == null) return false;
            return model.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 || model.Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            FilterBox.Focus();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (e.OriginalSource == this)
            {
                FilterBox.Focus();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Handled == false)
            {
                if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    Close();
                }
                else if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    Select();
                }
            }
        }

        private void BorderIconButton_Clicked(object obj)
        {
            Select();
        }

        private void Select()
        {
            var index = Display.SelectedItem;
            if (index == null) return;
            SelectModel(index as Model);
        }

        private Model GetFirstItem()
        {
            if (Display.ItemContainerGenerator.Items.Count > 0)
            {
                return Display.ItemContainerGenerator.Items[0] as Model;
            }
            return null;
        }

        private void FilterBox_EnterPressed(object sender, EventArgs e)
        {
            var selected = Display.SelectedItem as Model;
            if (selected == null)
            {
                selected = GetFirstItem();
            }
            SelectModel(selected);
        }


        private void SelectModel(Model model)
        {
            if (model != null)
            {
                SelectedType = model.type;
                if (SelectedType == null)
                {
                    return;
                }
                else
                {
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
