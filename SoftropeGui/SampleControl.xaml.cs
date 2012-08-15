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
using Teknohippy.Softrope;

namespace SoftropeGui
{
    /// <summary>
    /// Interaction logic for SampleControl.xaml
    /// </summary>
    /// 

    

    public partial class SampleControl : UserControl
    {
        public SampleControl(Sample sample, SoundEffect soundEffect)
        {
            this.InitializeComponent();
            Sample = sample;
            SoundEffect = soundEffect;
            SampleNameText.Text = Sample.Name;
            SampleWeightSlider.Value = Sample.Weight;
            SampleVolumeSlider.Value = Sample.Volume;
            
        }

        public SampleControl()
            : this(new Sample(), new SoundEffect())
        {

        }

        public Sample Sample { get; set; }
        public SoundEffect SoundEffect { get; set; }

        private void DeleteSampleButton_Click(object sender, RoutedEventArgs e)
        {
            OnRemoved(EventArgs.Empty);
        }

        public event EventHandler Removed;

        protected virtual void OnRemoved(EventArgs e)
        {
            if (Removed != null)
            {
                Removed(this, e);
            }
        }

        public event EventHandler LoadingSample;

        protected virtual void OnLoadingSample(EventArgs e)
        {
            if (LoadingSample != null)
            {
                LoadingSample(this, e);
            }

        }

        private void OpenSampleButton_Click(object sender, RoutedEventArgs e)
        {
            OnLoadingSample(EventArgs.Empty);
            Microsoft.Win32.OpenFileDialog openFd = new Microsoft.Win32.OpenFileDialog();
            openFd.InitialDirectory = Properties.Settings.Default.LastSamplePath;
            openFd.Filter = "Compatible Sound files|*.wav;*.mp3;*.wma;*.ogg;*.mp1;*.mp2;*.aiff";
            openFd.Filter += "|Wave Files (*.wav)|*.wav";
            openFd.Filter += "|Windows Media Files (*.wma)|*.wma";
            openFd.Filter += "|Ogg Files (*.ogg)|*.ogg";
            openFd.Filter += "|MP3 Files (*.mp3)|*.mp3";
            openFd.Filter += "|MP2 Files (*.mp2)|*.mp2";
            openFd.Filter += "|MP1 Files (*.mp1)|*.mp1";
            openFd.Filter += "|AIFF Files (*.aiff)|*.aiff";

            if ((bool)openFd.ShowDialog())
            {
                Sample.FileName = openFd.FileName;
                //SoundEffect.LoadSample(Sample.FileName, Sample);
                SampleNameText.Text = System.IO.Path.GetFileName(Sample.FileName);
                Properties.Settings.Default.LastSamplePath = System.IO.Path.GetDirectoryName(openFd.FileName);
            }
        }

        private void SampleNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sample != null)
            {
                Sample.Name = SampleNameText.Text; 
            }
        }

        private void SampleWeightSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Sample != null)
            {
                Sample.Weight = (float)((Slider)sender).Value;                
            }
        }

        private void SampleVolumeSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Sample != null)
            {
                Sample.Volume = (float)((Slider)sender).Value;
            }
        }

        private void SampleWeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Sample != null)
            {
                Sample.Weight = (float)((Slider)sender).Value;
                //Math not correct here.
                SampleWeightSlider.ToolTip = String.Format("{0:F0}%", (Sample.Weight / SoundEffect.TotalWeight) * 100);
            }
        }

        private void SampleWeightSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SampleWeightSlider.ToolTip = "Slide to set the probability of this sample being chosen randomly.";
        }



    }


}