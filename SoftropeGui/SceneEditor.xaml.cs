using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Teknohippy.Softrope;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace SoftropeGui
{
    public partial class SceneEditor
    {
        public SceneEditor(Scene sceneToEdit)
        {
            this.InitializeComponent();
            Scene = sceneToEdit;

            foreach (SoundEffect soundEffect in Scene.SoundEffects)
            {
                AddSoundEffectControl(soundEffect);
            }

            SceneNameTextBox.Text = Scene.Name;
            StopOthersCheckBox.IsChecked = Scene.StopsOthers;
            FadeOutCheckBox.IsChecked = Scene.FadesOut;
            PlayStopButton.IsChecked = Scene.IsPlaying;
            FadeOutSlider.Value = Scene.FadeOutLength;
            SceneVolumeSlider.Value = Scene.SceneVolume;
            FadeOutSlider.IsEnabled = Scene.FadesOut;
            

            AttachEvents();
        }

        private void AddSoundEffectControl(SoundEffect soundEffect)
        {
            SoundEffectControl soundEffectControl = new SoundEffectControl(soundEffect);
            SoundEffectsStackPanel.Children.Add(soundEffectControl);
            soundEffectControl.Removed += new EventHandler(soundEffectControl_Removed);
        }

        void soundEffectControl_Removed(object sender, EventArgs e)
        {
            if (Scene.SoundEffects.Count > 1)
            {
                SoundEffectControl soundEffectControl = sender as SoundEffectControl;
                Scene.SoundEffects.Remove(soundEffectControl.SoundEffect);
                SoundEffectsStackPanel.Children.Remove(soundEffectControl);
            }
        }

        private void AttachEvents()
        {
            Scene.Playing += new EventHandler(Scene_Playing);
            Scene.Stopping += new EventHandler(Scene_Stopping);
        }

        void Scene_Stopping(object sender, EventArgs e)
        {
            Scene.Stopping -= new EventHandler(Scene_Stopping);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (PlayStopButton.IsChecked == true)
                    PlayStopButton.IsChecked = false;

            }));
            Scene.Stopping += new EventHandler(Scene_Stopping);
        }

        void Scene_Playing(object sender, EventArgs e)
        {
            Scene.Playing -= new EventHandler(Scene_Playing);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (PlayStopButton.IsChecked == false)
                    PlayStopButton.IsChecked = true;
            }));
            Scene.Playing += new EventHandler(Scene_Playing);
        }

        public Scene Scene { get; set; }

        private void AddSoundEffectButton_Click(object sender, RoutedEventArgs e)
        {
            SoundEffect soundEffect = new SoundEffect();
            Scene.SoundEffects.Add(soundEffect);
            AddSoundEffectControl(soundEffect);
        }

        private void PlayStopButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PlayStopButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void PlayStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Scene.IsPlaying)
            {
                Scene.Stop();
            }
            else if (!Scene.IsPlaying || Scene.IsFading)
            {
                Scene.Play();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Scene.Stop(false);
        }

        private void SceneNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Scene != null)
            {
                Scene.Name = SceneNameTextBox.Text;
                this.Title = "Scene Editor : " + Scene.Name;
            }

        }

        private void StopOthersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Scene.StopsOthers == false && Scene != null)
            {
                Scene.StopsOthers = true;
            }
        }

        private void StopOthersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Scene.StopsOthers == true && Scene != null)
            {
                Scene.StopsOthers = false;
            }
        }

        private void FadeOutCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Scene.FadesOut == false)
            {
                Scene.FadesOut = true;
                ChangeFadeOutText();
                FadeOutSlider.IsEnabled = true;
            }
        }

        private void FadeOutCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Scene.FadesOut == true)
            {
                Scene.FadesOut = false;
                ChangeFadeOutText();
                FadeOutSlider.IsEnabled = false;
            }
        }

        private void FadeOutSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Scene != null)
            {
                Scene.FadeOutLength = (int)FadeOutSlider.Value;
                ChangeFadeOutText();
                FadeOutSlider.SelectionEnd = FadeOutSlider.Value;
            }
        }

        private void ChangeFadeOutText()
        {
            if (FadeStatusText != null)
            {
                string statusText = string.Empty;
                if (Scene.FadeOutLength == 0 || Scene.FadesOut == false)
                {
                    statusText = "No fade.";
                }
                else
                {
                    statusText = String.Format("Fade out for {0:F1} seconds.", Scene.FadeOutLength / 1000);
                }
                FadeStatusText.Text = statusText;
            }
        }

        private void SceneVolumeSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (Scene != null)
            {
                Scene.SceneVolume = (float)SceneVolumeSlider.Value;
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.PageDown:
                    SfxScroller.PageDown();
                    break;
                case Key.PageUp:
                    SfxScroller.PageUp();
                    break;
                case Key.Up:
                    SfxScroller.LineUp();
                    break;
                case Key.Down:
                    SfxScroller.LineDown();
                    break;
                case Key.Home:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        SfxScroller.ScrollToTop();
                    break;
                case Key.End:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        SfxScroller.ScrollToBottom();
                    break;
                default:
                    break;

            }
        }


    }
}