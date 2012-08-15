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
    public partial class LevelMeter
    {
        public LevelMeter()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.

            LedArray = new SingleLed[10];

            for (int i = 0; i < 10; i++)
            {
                LedArray[i] = new SingleLed();
                LayoutRoot.Children.Add(LedArray[i]);
            }

            LedArray[0].LedColor = LedColors.Green;
            LedArray[1].LedColor = LedColors.Green;
            LedArray[2].LedColor = LedColors.Green;
            LedArray[3].LedColor = LedColors.Green;
            LedArray[4].LedColor = LedColors.Green;
            LedArray[5].LedColor = LedColors.Orange;
            LedArray[6].LedColor = LedColors.Orange;
            LedArray[7].LedColor = LedColors.Orange;
            LedArray[8].LedColor = LedColors.Red;
            LedArray[9].LedColor = LedColors.Red;
            

            level = 0;
        }

        private SingleLed[] LedArray;
        private int level;

        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value == level || value > 10 || value < 0)
                {
                    return;
                }
                else if (value > level)
                {
                    for (int i = level + 1; i <= value; i++)
                    {
                        LedArray[i - 1].LedOn = true;
                    }
                }
                else if (value < level)
                {
                    for (int i = level; i > value; i--)
                    {
                        LedArray[i - 1].LedOn = false;
                    }
                }

                level = value;
            }
        }

        public int Volume
        {
            set
            {
                if (value > 0 && value < 128)
                {
                    Level = (int)(value / 12.7);
                }
            }
        }

    }
}