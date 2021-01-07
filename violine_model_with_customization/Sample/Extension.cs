using Sample;
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

namespace Sample1
{
    public class BoxAndWhiskerSeriesExt : BoxAndWhiskerSeries
    {
        public override void CreateSegments()
        {
            base.CreateSegments();

            var segments = new ObservableCollection<ChartSegment>();
            
            foreach (var segment in Segments)
            {
                if (segment is BoxAndWhiskerSegment)
                {
                    var item = segment as BoxAndWhiskerSegment;

                    var boxSegment = new BoxAndWhiskerSegmentExt(this);

                    boxSegment.SetData((double)ReflectionExt.GetInternalProperty("Left", item),
                       (double)ReflectionExt.GetInternalProperty("Right", item),
                        (double)ReflectionExt.GetInternalProperty("Bottom", item),
                        item.Minimum,
                        item.LowerQuartile,
                        item.Median,
                        item.UppperQuartile,
                        item.Maximum,
                       (double)ReflectionExt.GetInternalProperty("Top", item),
                       (double)ReflectionExt.GetInternalProperty("Center", item),
                       (double)ReflectionExt.GetInternalField("average", item, typeof(BoxAndWhiskerSegment)));

                    boxSegment.Item = item.Item;
                    ReflectionExt.SetInternalProperty("Outliers", item, boxSegment);
                    ReflectionExt.SetInternalProperty("WhiskerWidth", item, boxSegment);

                    segments.Add(boxSegment);
                }
            }

            Segments.Clear();

            foreach (var item in segments)
            {
                Segments.Add(item);
            }
        }
    }


    public class BoxAndWhiskerSegmentExt : BoxAndWhiskerSegment
    {
        internal double Left { get; set; }
        internal double Right { get; set; }
        internal double Center { get; set; }
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

        Path violinPath;
        public override UIElement CreateVisual(Size size)
        {
            var element = base.CreateVisual(size);

            violinPath = new Path();
            violinPath.Tag = this;
            violinPath.Stretch = Stretch.Fill;
            violinPath.Fill = Interior;
            SetVisualBindings(violinPath);

            (element as Canvas).Children.Insert(0,violinPath);

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

            string data = "M " + maximumPoint.X + "," + maximumPoint.Y
                + " C " + (brPoint.X - (maximumPoint.X - tlPoint.X) / 2 - extensionValue) + "," + (tlPoint.Y - (tlPoint.Y - maximumPoint.Y) / 2)
                + " " + brPoint.X + "," + tlPoint.Y
                + " " + brPoint.X + "," + medianPoint.Y
                + " C " + brPoint.X + "," + brPoint.Y
                + " " + (brPoint.X - (centerLinePosition - tlPoint.X) / 2 - extensionValue) + "," + (brPoint.Y + (minimumPoint.Y - brPoint.Y) / 2)
                + " " + centerLinePosition + "," + (minimumPoint.Y + (extensionValue * 2))
                + " C " + (tlPoint.X + (centerLinePosition - tlPoint.X) / 2 + extensionValue) + "," + (brPoint.Y + (minimumPoint.Y - brPoint.Y) / 2)
                + " " + tlPoint.X + "," + brPoint.Y
                + " " + tlPoint.X + "," + medianPoint.Y
                + " C " + tlPoint.X + "," + tlPoint.Y
                + " " + (tlPoint.X + (maximumPoint.X - tlPoint.X) / 2 + extensionValue) + "," + (tlPoint.Y - (tlPoint.Y - maximumPoint.Y) / 2)
                + " " + maximumPoint.X + "," + maximumPoint.Y;

            violinPath.Data = Geometry.Parse(data);
        }
    }
}
