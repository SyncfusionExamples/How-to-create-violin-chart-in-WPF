# How to create violin chart in WPF (SfChart)

A Violin Plot is used to visualise the distribution of the data and its probability density. It has been achieved by customizing the [BoxAndWhiskerSeries](https://help.syncfusion.com/wpf/charts/seriestypes/other#box-and-whisker) in [WPF Chart](https://help.syncfusion.com/wpf/charts/overview). 


A Violin Plot (Hybrid of Box Plot Chart) is used to visualize the distribution of the data and its probability density. [WPF Charts](https://help.syncfusion.com/wpf/charts/overview) provides support to draw violin basic model by customizing the [BoxAndWhiskerSeries](https://help.syncfusion.com/wpf/charts/seriestypes/other#box-and-whisker) segment rendering.

## Basic Violin Plot
The following steps are explaining how to create a basic violin plot using the Box and Whisker Chart in WPF.

**Step 1:**  Create an extended Box Plot Series (BoxAndWhiskerSeriesExt) and override its default segment with an extended Box Plot (BoxAndWhiskerSegmentExt) Segment as shown below.

```
public class BoxAndWhiskerSeriesExt : BoxAndWhiskerSeries
    {
        protected override ChartSegment CreateSegment()
        {
            return new BoxAndWhiskerSegmentExt(this);
        }
    }
```

**Step 2:**  In extended Box Plot Segment involves the appearance changing by using the CreateVisual method which is used to create a violin path.
```
public class BoxAndWhiskerSegmentExt : BoxAndWhiskerSegment
{
    …
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
}
```

**Step3:** The Geometry path of the violin plot has been created and update as violinPath in the Update method as shown below.
```
public class BoxAndWhiskerSegmentExt : BoxAndWhiskerSegment
{
    …
    public override void Update(IChartTransformer transformer)
    {
        base.Update(transformer);

        BoxAndWhiskerSegment baseSegment = this as BoxAndWhiskerSegment;
        
        …
        //Right side path
        BezierSegment segment1 = new BezierSegment();
        segment1.Point1 = new Point(centerLinePosition, minimumPoint.Y + (extensionValue * 2));
        segment1.Point2 = new Point(tlPoint.X + (centerLinePosition - tlPoint.X) / 2 + extensionValue, brPoint.Y + (minimumPoint.Y - brPoint.Y) / 2);
        segment1.Point3 = new Point(tlPoint.X, brPoint.Y);
        pathFigure.Segments.Add(segment1);
        …

        PathGeometry geometry = new PathGeometry();
        geometry.Figures.Add(pathFigure);
        violinPath.Data = geometry;
    }
}
```

**Step4:** This is for finding how to define in XAML to get in visual.
```
<chart:SfChart x:Name="boxWhiskerChart" AreaBorderBrush="#8e8e8e" 
                       Background="White" Margin="10,20,20,20" 
                       VerticalAlignment="Bottom" AreaBorderThickness="0,1,1,1">
	…
    <local:BoxAndWhiskerSeriesExt ItemsSource="{Binding BoxWhiskerData}"  
                                       XBindingPath="Department"   ShowOutlier="False"
                                       YBindingPath="Age" ShowTooltip="True"
                                       BoxPlotMode="Normal"
                                       x:Name="boxSeries"/>
</chart:SfChart>
```

![Violin Chart WPF](https://github.com/SyncfusionExamples/How-to-create-violin-chart-in-WPF/blob/main/Violin-Chart.png)

## Violin Plot Customization
To obtain the smooth edges and interior filling in the violin chart, use the pre-defined values for minimum, maximum, and median points with the provided SVG path as shown below.

```
public class BoxAndWhiskerSegmentExt : BoxAndWhiskerSegment
{
    …
    public override void Update(IChartTransformer transformer)
    {
        base.Update(transformer);

        …
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

```

![Customized Violin Chart WPF](https://github.com/SyncfusionExamples/How-to-create-violin-chart-in-WPF/blob/main/Violin-Chart-Customization.png)


**KB article** - [How to create violin chart in WPF](https://www.syncfusion.com/kb/12489/how-to-create-violin-chart-in-wpf)

## See also

[How to customize the default shape of any series with the required shapes](https://www.syncfusion.com/kb/3853/how-to-customize-the-default-shape-of-any-series-with-the-required-shapes?)

[How to create the Tornado Chart in WPF Charts](https://www.syncfusion.com/kb/11657/how-to-create-the-tornado-chart-in-wpf-charts?)

[How to customize the outlier of Box Plot in WPF Charts](https://help.syncfusion.com/wpf/charts/seriestypes/other#outlier)
