using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoftropeGui
{
	/// <summary>
	/// Interaction logic for Temp.xaml
	/// </summary>
	public partial class Temp : Window
	{
		public Temp()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SceneControl sc = new SceneControl();
            IainList.Items.Add(sc);            
        }
	}
}