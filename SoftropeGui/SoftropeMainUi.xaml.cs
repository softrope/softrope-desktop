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
using System.IO;

namespace SoftropeGui
{
    /// <summary>
    /// Interaction logic for SoftropeMainUi.xaml
    /// </summary>
    public partial class SoftropeMainUi : Window
    {

        public SoftropeMainUi()
        {
            this.InitializeComponent();
            Softrope.InitialiseBass();
            Softrope.SetBuffer();
            Module = new Module();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(GetType());
            Status.Text = String.Format("{0}. Version {1}.",
                assembly.GetName().Name,
                assembly.GetName().Version
                );
        }

        private void AddSceneButton_Click(object sender, RoutedEventArgs e)
        {
            Scene scene = new Scene();
            Module.Scenes.Add(scene);
            AddSceenControl(scene);
            SoundEffect soundEffect = new SoundEffect();
            scene.SoundEffects.Add(soundEffect);
            Sample sample = new Sample();
            soundEffect.Samples.Add(sample);
            Module.ReverbTime = 0;
            Module.ReverbMix = 0;
        }

        private SceneControl AddSceenControl(Scene scene)
        {
            SceneControl sceneControl = new SceneControl(scene);
            ScenesWrapPanel.Children.Add(sceneControl);
            sceneControl.Removed += new EventHandler(sceneControl_Removed);            
            return sceneControl;
        }

        
        void sceneControl_Removed(object sender, EventArgs e)
        {
            SceneControl sceneControl = sender as SceneControl;
            sceneControl.Scene.Stop(false);
            Module.Scenes.Remove(sceneControl.Scene);
            ScenesWrapPanel.Children.Remove(sceneControl);
        }

 
        public Module Module { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Softrope.UnloadBass();
            foreach (Window window in Application.Current.Windows)
            {
                if (window != sender)
                {
                    window.Close();
                }
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            Microsoft.Win32.SaveFileDialog saveFd = new Microsoft.Win32.SaveFileDialog();
            saveFd.InitialDirectory = Properties.Settings.Default.LastSavePath;
            saveFd.Filter = "Softrope Module (*.softrope)|*.softrope";

            if ((bool)saveFd.ShowDialog())
            {
                Softrope.Save(Module, saveFd.FileName);
                MessageBox.Show("Saved");
                Module.IsDirty = false;
                Properties.Settings.Default.LastSavePath = System.IO.Path.GetDirectoryName(saveFd.FileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFd = new Microsoft.Win32.OpenFileDialog();
            openFd.InitialDirectory = Properties.Settings.Default.LastSavePath;
            openFd.Filter = "Sofrope Module (*.softrope)|*.softrope";

            if ((bool)openFd.ShowDialog())
            {
                OpenFile(openFd.FileName);
            }
        }

        public void OpenFile(String fileName)
        {
            Module.Scenes.Clear();
            Module = Softrope.Load(Module, fileName);
            SyncUi();
            Properties.Settings.Default.LastSavePath = System.IO.Path.GetDirectoryName(fileName);
        }

        private void SyncUi()
        {
            ScenesWrapPanel.Children.Clear();
            foreach (Scene scene in Module.Scenes)
            {
                SceneControl sceneControl = AddSceenControl(scene);

                sceneControl.SceneNameTextBox.Text = scene.Name;

                if (scene.ButtonImage != null)
                {
                    Image newImage = new Image();
                    newImage.Width = 64;
                    newImage.Height = 64;

                    MemoryStream ms = new MemoryStream(scene.ButtonImage);

                    BitmapImage newBitmap = new BitmapImage();
                    newBitmap.BeginInit();
                    newBitmap.StreamSource = ms;
                    newBitmap.DecodePixelHeight = 64;
                    newBitmap.DecodePixelWidth = 64;
                    newBitmap.EndInit();

                    newImage.Source = newBitmap;

                    sceneControl.ButtonImage = newImage;
                }
            }
        }


        private void ScenesWrapPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SceneControl sceneControl = e.Source as SceneControl;
            if (sceneControl != null)
            {
                startPosition = e.GetPosition(null);
            }
        }

        private Point startPosition;

        private void ScenesWrapPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(TextBox))
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(null);
                Vector dragVector = startPosition - currentPosition;
                
                if (Math.Abs(dragVector.Length) > 5)
                {
                    SceneControl sceneControl = e.Source as SceneControl;
                    if (sceneControl != null)
                    {
                        DataObject dataObject = new DataObject("SceneControl", sceneControl);

                        try
                        {
                            DragDrop.DoDragDrop(sceneControl, dataObject, DragDropEffects.Move);
                        }
                        catch
                        {

                        }                        
                    }
                }
            }
        }

        private void ScenesWrapPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void ScenesWrapPanel_Drop(object sender, DragEventArgs e)
        {
            SceneControl dropsc = e.Source as SceneControl;
            SceneControl sc = e.Data.GetData("SceneControl") as SceneControl;
            if (dropsc == null)
            {
                ScenesWrapPanel.Children.Add(sc);
                Module.Scenes.Add(sc.Scene);
            }
        }

        private void ScenesWrapPanel_DragOver(object sender, DragEventArgs e)
        {
            SceneControl dropsc = e.Source as SceneControl;
            SceneControl sc = e.Data.GetData("SceneControl") as SceneControl;
            if (dropsc != null && dropsc != sc)
            {
                Int32 dropIndex = dropIndex = ScenesWrapPanel.Children.IndexOf(dropsc);

                try
                {
                    ScenesWrapPanel.Children.Remove(sc);
                    Module.Scenes.Remove(sc.Scene);
                }
                catch { }

                ScenesWrapPanel.Children.Insert(dropIndex, sc);
                Module.Scenes.Insert(dropIndex, sc.Scene);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.PageDown:
                    SceneScroller.PageDown();
                    break;
                case Key.PageUp:
                    SceneScroller.PageUp();
                    break;
                case Key.Up:
                    SceneScroller.LineUp();
                    break;
                case Key.Down:
                    SceneScroller.LineDown();
                    break;
                case Key.Home:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        SceneScroller.ScrollToTop();
                    break;
                case Key.End:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        SceneScroller.ScrollToBottom();
                    break;
                default:
                    break;

            }
        }

        private void ReverbKnob_ValueChanged(object sender, EventArgs e)
        {
            Module.ReverbTime = (float)ReverbKnob.Value;
            Module.ReverbMix = (float)MixKnob.Value;
        }

    }
}