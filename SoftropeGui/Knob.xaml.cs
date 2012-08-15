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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftropeGui
{
    /// <summary>
    /// Interaction logic for Knob.xaml
    /// </summary>
    public partial class Knob
    {
        public Knob()
        {
            this.InitializeComponent();
            SetAngle();
            this.Maximum = 100;
            this.Minimum = 0;
        }

        private bool userChangingValue = false;

        private bool userDragged = false;

        private Point startPosition;

        private Double startAngle = -150;

        private RotateTransform rotateTransform = new RotateTransform(-150);

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.userChangingValue = true;
            this.startPosition = e.GetPosition(null);
            Mouse.Capture(LayoutRoot);
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.userChangingValue = false;
            Mouse.Capture(null);
            

            if (this.userDragged == false)
            {
                SetByClick(e);
            }
            this.userDragged = false;
            this.startAngle = rotateTransform.Angle;
        }

        private void SetByClick(MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(KnobPart);
            clickPosition.X = clickPosition.X - 16;
            clickPosition.Y = 16 - clickPosition.Y;
            Double angle = RadianToDegree(Math.Atan(clickPosition.X / clickPosition.Y));
            if (clickPosition.X < 0 && clickPosition.Y < 0)
            {
                angle = angle - 180;
            }
            if (clickPosition.X > 0 && clickPosition.Y < 0)
            {
                angle = angle + 180;
            }
            rotateTransform.Angle = angle;
            SetAngle();
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.userChangingValue == true)
            {
                if (this.startPosition != null)
                {
                    Vector delta = this.startPosition - e.GetPosition(null);
                    if (delta.Length > 0) this.userDragged = true;
                    if (Math.Abs(delta.X) > Math.Abs(delta.Y))
                    {
                        rotateTransform.Angle = this.startAngle - 2 * delta.X;
                    }
                    else
                    {
                        rotateTransform.Angle = this.startAngle + 2 * delta.Y;
                    }                    
                    SetAngle();
                }

            }
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            LayoutRoot_MouseMove(sender, e);
        }

        private Double GetInternalValue()
        {
            return (rotateTransform.Angle + 150) / 300;
        }

        public Double Minimum { get; set; }
        public Double Maximum { get; set; }

        private Double value;

        public Double Value
        {
            get
            {
                Double valueRange = this.Maximum - this.Minimum;
                this.value = valueRange * this.GetInternalValue() + this.Minimum;
                return this.value;
            }
            set
            {
                this.value = value;
                SetInternalValue();
            }

        }

        private void SetInternalValue()
        {
            Double valueRange = this.Maximum - this.Minimum;
            Double zeroedValue = this.value - this.Minimum;
            Double internalValue = zeroedValue / valueRange;
            rotateTransform.Angle = (internalValue * 300) - 150;
            SetAngle();
        }

        private void SetAngle()
        {
            if (rotateTransform.Angle > 150)
            {
                rotateTransform.Angle = 150;
            }
            if (rotateTransform.Angle < -150)
            {
                rotateTransform.Angle = -150;
            }
            CapLine.RenderTransform = rotateTransform;

            OnValueChanged(EventArgs.Empty);
        }

        private void LayoutRoot_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            rotateTransform.Angle = rotateTransform.Angle + (e.Delta/10);
            SetAngle();
            this.startAngle = rotateTransform.Angle;
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
                this.ToolTip = Value;
            }
        }








    }
}