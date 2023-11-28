using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDrawing
{
    public class CustomDrawableView : IDrawable
    {
        DrawingBox MainView;

        public CustomDrawableView(DrawingBox MainView)
        {
            this.MainView = MainView;

        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (MainView.ContainerBackground is SolidColorBrush solidColorBrush)
            {
                canvas.FillColor = solidColorBrush.Color;
            }
            canvas.FillEllipse(10, 10, 150, 50);
        }
    }
}
