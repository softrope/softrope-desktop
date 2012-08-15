using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SoftropeGui
{
    public class TooltipSlider : Slider
    {
        private ToolTip autoToolTip;

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.AutoToolTip.Content = this.ToolTip;
        }

        private ToolTip AutoToolTip
        {
            get
            {
                if (autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField(
                        "_autoToolTip",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    autoToolTip = field.GetValue(this) as ToolTip;
                }

                return autoToolTip;
            }
        }
    }
}
