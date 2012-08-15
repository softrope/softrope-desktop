using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Teknohippy.Softrope;
using System.Windows.Threading;

namespace SoftropeGui
{
    public partial class SceneControl
    {
        public SceneControl(Scene scene)
        {
            this.InitializeComponent();
            Scene = scene;
            SceneNameTextBox.Text = scene.Name;
            Scene.Playing += new EventHandler(Scene_Playing);
            Scene.Stopping += new EventHandler(Scene_Stopping);
        }

        public SceneControl()
            : this(new Scene())
        {

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

        private SceneEditor sceneEditor;

        public Scene Scene { get; set; }

        public Image ButtonImage
        {
            get
            {
                return PlayStopButton.Content as Image;
            }
            set
            {
                PlayStopButton.Content = value;
            }
        }

        void sceneEditor_Closed(object sender, EventArgs e)
        {
            sceneEditor = null;
            SceneNameTextBox.Text = Scene.Name;
        }

        private void EditSceneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditScene();
        }

        private void EditScene()
        {
            Scene.Stop(false);
            if (sceneEditor == null)
            {
                sceneEditor = new SceneEditor(Scene);
                sceneEditor.Closed += new EventHandler(sceneEditor_Closed);
            }
            sceneEditor.Show();
            sceneEditor.Focus();
        }

        private void PlayStopButton_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void PlayStopButton_Checked(object sender, RoutedEventArgs e)
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

        private void FadeOutMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (Scene.FadesOut == false)
            {
                Scene.FadesOut = true;
            }
        }

        private void FadeOutMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Scene.FadesOut == true)
            {
                Scene.FadesOut = false;
            }
        }

        private void StopsOthersMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (Scene.StopsOthers == false)
            {
                Scene.StopsOthers = true;
            }
        }

        private void StopsOthersMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Scene.StopsOthers == true)
            {
                Scene.StopsOthers = false;
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            FadeOutMenuItem.IsChecked = Scene.FadesOut;
            StopsOthersMenuItem.IsChecked = Scene.StopsOthers;
        }

        private void ButtonNameText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoadIconMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFd = new Microsoft.Win32.OpenFileDialog();
            openFd.InitialDirectory = Properties.Settings.Default.LastImagePath;
            openFd.Filter = "Image files (*.png, *.gif, *.jpg, *.bmp)|*.png;*.gif;*.jpg;*.bmp";

            if ((bool)openFd.ShowDialog())
            {
                Scene.ButtonImage = File.ReadAllBytes(openFd.FileName);

                Image newImage = new Image();
                newImage.Width = 64;
                newImage.Height = 64;

                MemoryStream ms = new MemoryStream(Scene.ButtonImage);

                BitmapImage newBitmap = new BitmapImage();
                newBitmap.BeginInit();
                newBitmap.StreamSource = ms;
                newBitmap.DecodePixelHeight = 64;
                newBitmap.DecodePixelWidth = 64;
                newBitmap.EndInit();

                newImage.Source = newBitmap;

                ButtonImage = newImage;

                Properties.Settings.Default.LastImagePath = Path.GetDirectoryName(openFd.FileName);
            }
        }

        private void SceneNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Scene != null)
            {
                Scene.Name = SceneNameTextBox.Text;
            }
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            DropState = DropState.None;
        }

        public DropState DropState
        {
            set
            {
                switch (value)
                {
                    case DropState.Left:
                        LeftLine.Visibility = Visibility.Visible;
                        RightLine.Visibility = Visibility.Collapsed;
                        break;
                    case DropState.Right:
                        RightLine.Visibility = Visibility.Visible;
                        LeftLine.Visibility = Visibility.Collapsed;
                        break;
                    case DropState.None:
                        LeftLine.Visibility = Visibility.Collapsed;
                        RightLine.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e)
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

        private void PlayStopButton_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditScene();
            e.Handled = true;
        }

        private void ExportZipItem_Click(object sender, RoutedEventArgs e)
        {
            Softrope.SaveSceneZip(this.Scene, "d:\\test.zip");
            MessageBox.Show("Exported");
        }








    }

    public enum DropState
    {
        Left,
        Right,
        None
    }
}