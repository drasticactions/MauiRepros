using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDrawing
{
    public class DrawingBox : GraphicsView
    {

        public static readonly BindableProperty ContainerBackgroundProperty = BindableProperty.Create("ContainerBackground", typeof(Brush), typeof(DrawingBox), new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromRgba("E7E0EC")), BindingMode.Default, null, OnPropertyChanged);

        public Brush ContainerBackground
        {
            get
            {
                return (Brush)GetValue(ContainerBackgroundProperty);
            }
            set
            {
                SetValue(ContainerBackgroundProperty, value);
            }
        }

        public DrawingBox()
        {
            this.Drawable = new CustomDrawableView(this);
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as DrawingBox)?.Invalidate();
        }


    }
}
