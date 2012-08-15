using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace SoftropeGui
{
    internal static class SharedDictionaryManager
    {
        internal static ResourceDictionary SharedDictionary
        {
            get
            {
                if (sharedDictionary == null)
                {
                    System.Uri resourceLocater =
                        new System.Uri("/SoftropeGui;component/LedBrushesDictionary.xaml",
                                        System.UriKind.Relative);

                    sharedDictionary =
                        (ResourceDictionary)Application.LoadComponent(resourceLocater);
                }

                return sharedDictionary;
            }
        }

        private static ResourceDictionary sharedDictionary;
    }

    public enum LedColors
    {
        Blue = 0,
        Green = 1,
        Orange = 2,
        Red = 3
    }

    public partial class SingleLed
    {
        public SingleLed()
        {
            this.Resources.MergedDictionaries.Add(SharedDictionaryManager.SharedDictionary);

            blueOn = this.Resources["blueOn"] as RadialGradientBrush;
            blueOff = this.Resources["blueOff"] as RadialGradientBrush;
            greenOn = this.Resources["greenOn"] as RadialGradientBrush;
            greenOff = this.Resources["greenOff"] as RadialGradientBrush;
            orangeOn = this.Resources["orangeOn"] as RadialGradientBrush;
            orangeOff = this.Resources["orangeOff"] as RadialGradientBrush;
            redOn = this.Resources["redOn"] as RadialGradientBrush;
            redOff = this.Resources["redOff"] as RadialGradientBrush;

            this.InitializeComponent();


        }

        private static RadialGradientBrush blueOn;
        private static RadialGradientBrush blueOff;
        private static RadialGradientBrush greenOn;
        private static RadialGradientBrush greenOff;
        private static RadialGradientBrush orangeOn;
        private static RadialGradientBrush orangeOff;
        private static RadialGradientBrush redOn;
        private static RadialGradientBrush redOff;

        private LedColors ledColor;

        private bool ledOn;

        private RadialGradientBrush myOnBrush;

        private RadialGradientBrush myOffBrush;

        public LedColors LedColor
        {
            get
            {
                return ledColor;
            }
            set
            {
                ledColor = value;

                switch (value)
                {
                    case LedColors.Blue:
                        myOnBrush = blueOn;
                        myOffBrush = blueOff;
                        break;
                    case LedColors.Green:
                        myOnBrush = greenOn;
                        myOffBrush = greenOff;
                        break;
                    case LedColors.Orange:
                        myOnBrush = orangeOn;
                        myOffBrush = orangeOff;
                        break;
                    case LedColors.Red:
                        myOnBrush = redOn;
                        myOffBrush = redOff;
                        break;
                    default:
                        myOnBrush = blueOn;
                        myOffBrush = blueOff;
                        break;
                }

                LedOn = ledOn;
            }
        }

        public bool LedOn
        {
            get
            {
                return ledOn;
            }
            set
            {
                if (value == true)
                {
                    Diode.Fill = myOnBrush;
                    ledOn = true;
                }
                else
                {
                    try
                    {
                        Diode.Fill = myOffBrush;
                        ledOn = false;
                    }
                    catch
                    {

                    }
                }
            }
        }

    }
}