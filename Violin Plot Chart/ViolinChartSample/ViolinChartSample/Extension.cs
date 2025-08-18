using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ViolinChartSample
{
    public class BoxAndWhiskerSeriesExt : BoxAndWhiskerSeries
    {
        protected override ChartSegment CreateSegment()
        {
            return new BoxAndWhiskerSegmentExt(this);
        }
    }


    public class BoxAndWhiskerSegmentExt : BoxAndWhiskerSegment
    {
        internal double Left { get; set; }
        internal double Right { get; set; }
        internal double Center { get; set; }
        private Path violinPath;

        public BoxAndWhiskerSegmentExt(ChartSeriesBase series) : base(series)
        {

        }

        public override void SetData(params double[] Values)
        {
            base.SetData(Values);
            Left = Values[0];
            Right = Values[1];
            Center = Values[9];
        }

      
        public override UIElement CreateVisual(Size size)
        {
            var element = base.CreateVisual(size);
            violinPath = new Path()
            {
                Tag = this,
                Stretch = Stretch.Fill,
            };
         
            SetVisualBindings(violinPath);
            (element as Canvas).Children.Add(violinPath);
            return element;
        }

        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            
            BoxAndWhiskerSegment baseSegment = this as BoxAndWhiskerSegment;
            
            Line maximumLine = (Line)ReflectionExt.GetInternalField("maximumLine", baseSegment, typeof(BoxAndWhiskerSegment));
            Line minimumLine = (Line)ReflectionExt.GetInternalField("minimumLine", baseSegment, typeof(BoxAndWhiskerSegment));
            Rectangle rectangle = (Rectangle)ReflectionExt.GetInternalField("rectangle", baseSegment, typeof(BoxAndWhiskerSegment));
            Line medianLine = (Line)ReflectionExt.GetInternalField("medianLine", baseSegment, typeof(BoxAndWhiskerSegment));

            Point minimumPoint = transformer.TransformToVisible(Center, Minimum);
            Point medianPoint = transformer.TransformToVisible(Center, Median);
            Point maximumPoint = transformer.TransformToVisible(Center, Maximum);
            var tlPoint = transformer.TransformToVisible(Left, UppperQuartile);
            var brPoint = transformer.TransformToVisible(Right, LowerQuartile);
            var centre = tlPoint.X + (brPoint.X - tlPoint.X) / 2;
            var rect = new Rect(new Point(centre - 2.5, tlPoint.Y), new Point(centre + 2.5, brPoint.Y));
            var centerLinePosition = rect.X + rect.Width / 2;

            maximumLine.Visibility = Visibility.Collapsed;
            minimumLine.Visibility = Visibility.Collapsed;
            medianLine.Visibility = Visibility.Collapsed;
            
            double rectWidth = 5;
            rectangle.SetValue(Canvas.LeftProperty, rect.X + (rect.Width / 2) - (rectWidth / 2));
            rectangle.Width = rectWidth;

            double extensionValue = 20;
            Canvas.SetLeft(violinPath, tlPoint.X);
            Canvas.SetTop(violinPath, maximumPoint.Y - extensionValue);

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(centerLinePosition, minimumPoint.Y + (extensionValue * 2));

            //Right side path
            BezierSegment segment1 = new BezierSegment();
            segment1.Point1 = new Point(centerLinePosition, minimumPoint.Y + (extensionValue * 2));
            segment1.Point2 = new Point(tlPoint.X + (centerLinePosition - tlPoint.X) / 2 + extensionValue, brPoint.Y + (minimumPoint.Y - brPoint.Y) / 2);
            segment1.Point3 = new Point(tlPoint.X, brPoint.Y);
            pathFigure.Segments.Add(segment1);

            segment1 = new BezierSegment();
            segment1.Point1 = new Point(tlPoint.X, brPoint.Y);
            segment1.Point2 = new Point(tlPoint.X - (tlPoint.X - medianPoint.X), medianPoint.Y);
            segment1.Point3 = new Point(tlPoint.X, tlPoint.Y);
            pathFigure.Segments.Add(segment1);

            segment1 = new BezierSegment();
            segment1.Point1 = new Point(tlPoint.X, tlPoint.Y);
            segment1.Point2 = new Point(tlPoint.X + (maximumPoint.X - tlPoint.X) / 2 + extensionValue, tlPoint.Y - (tlPoint.Y - maximumPoint.Y) / 2);
            segment1.Point3 = new Point(maximumPoint.X, maximumPoint.Y);
            pathFigure.Segments.Add(segment1);

            //Left side path
            segment1 = new BezierSegment();

            segment1.Point1 = new Point(maximumPoint.X, maximumPoint.Y);
            segment1.Point2 = new Point(brPoint.X - (maximumPoint.X - tlPoint.X) / 2 - extensionValue, tlPoint.Y - (tlPoint.Y - maximumPoint.Y) / 2);
            segment1.Point3 = new Point(brPoint.X, tlPoint.Y);
            pathFigure.Segments.Add(segment1);

            segment1 = new BezierSegment();
            segment1.Point1 = new Point(brPoint.X, tlPoint.Y);
            segment1.Point2 = new Point(brPoint.X + (tlPoint.X - medianPoint.X), medianPoint.Y);
            segment1.Point3 = new Point(brPoint.X, brPoint.Y);
            pathFigure.Segments.Add(segment1);

            segment1 = new BezierSegment();
            segment1.Point1 = new Point(brPoint.X, brPoint.Y);
            segment1.Point2 = new Point(brPoint.X - (centerLinePosition - tlPoint.X) / 2 - extensionValue, brPoint.Y + (minimumPoint.Y - brPoint.Y) / 2);
            segment1.Point3 = new Point(centerLinePosition, minimumPoint.Y + (extensionValue * 2));
            pathFigure.Segments.Add(segment1);

            PathGeometry geometry = new PathGeometry();

            geometry.Figures.Add(pathFigure);
            violinPath.Data = geometry;

        }
    }
}
