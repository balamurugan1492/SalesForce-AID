/*
* =====================================
* Pointel.Interactions.TeamCommunicator.UserControls
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Pointel.Interactions.IPlugins;

namespace Pointel.Interactions.TeamCommunicator.UserControls
{
	/// <summary>
	/// Interaction logic for TeamCommunicator.xaml
	/// </summary>
	public partial class TeamCommunicator : UserControl
	{

		bool _isMenuEnabled = false;

		public TeamCommunicator(InteractionType interactionType,
			OperationType operationType, bool isMenuEnabled,
			Func<Dictionary<string, string>, string> refFunction)
		{
			InitializeComponent();
			_isMenuEnabled = isMenuEnabled;
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			grdContent.Visibility = Visibility.Visible;
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			grdContent.Visibility = Visibility.Collapsed;
		}

		private void grdContent_GotFocus(object sender, RoutedEventArgs e)
		{
			grdContent.Visibility = Visibility.Visible;
		}
	}
}
