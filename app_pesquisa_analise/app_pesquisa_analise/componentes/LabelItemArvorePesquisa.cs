﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class LabelItemArvorePesquisa : StackLayout
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create<LabelItemArvorePesquisa, ICommand>(p => p.Command, null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create<LabelItemArvorePesquisa, object>(p => p.CommandParameter, null);
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    /*this.AnchorX = 0.48;
                    this.AnchorY = 0.48;
                    await this.ScaleTo(0.8, 50, Easing.Linear);
                    await Task.Delay(50);
                    await this.ScaleTo(1, 50, Easing.Linear);*/
                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                    }
                });
            }
        }
        public void Initialize(String descricao, bool temFilhos)
        {
            Label label = new Label()
            {
                FontSize = 17,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromHex("#212121"),
                Text = descricao
            };
            
            if (temFilhos)
                label.FontAttributes = FontAttributes.Bold;

            Padding = new Thickness(0, 5, 0, 0);
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;

            Children.Add(label);

            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = TransitionCommand
            });
        }

        public LabelItemArvorePesquisa(String descricao, bool temFilhos)
        {
            Initialize(descricao, temFilhos);
        }
    }
}
