using System.Drawing;
using System.IO;
using System.Web.UI.DataVisualization.Charting;

namespace CRP.Core.Abstractions
{
    public interface IChartProvider
    {
        MemoryStream CreateChart(string[] xAxis, int[] yAxis, string title, SeriesChartType type);
    }

    public class ChartProvider : IChartProvider
    {
        /// <summary>
        /// Creates the actual chart and passes back the memory stream containing the chart.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public MemoryStream CreateChart(string[] xAxis, int[] yAxis, string title, SeriesChartType type)
        {
            // get a prepped chart
            var chart = PrepChart(title);

            // add in the data
            var series = chart.Series.Add("Series1");
            series.ChartType = type;
            series.YValueType = ChartValueType.Int32;
            series.XValueType = ChartValueType.String;
            series.BorderColor = Color.Azure;
            series.ShadowOffset = 2;
            series.Points.DataBindXY(xAxis, yAxis);

            // Save Chart to MemoryStream
            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);

            return ms;
        }

        /// <summary>
        /// Setup the basic settings for a chart, all that needs to be added is a series of data.
        /// </summary>
        /// <param name="titleText">The title you would like displayed in the graph</param>
        /// <returns></returns>
        private Chart PrepChart(string titleText)
        {
            var chart = new Chart();
            chart.Width = 412;
            chart.Height = 296;
            chart.ImageType = ChartImageType.Png;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BackColor = Color.Cornsilk;
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 2;
            chart.BorderlineColor = Color.Blue;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

            var title = chart.Titles.Add("Main");
            title.Text = titleText;
            title.Docking = Docking.Top;
            title.Font = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Bold);

            ChartArea chartArea = chart.ChartAreas.Add("Default");
            chartArea.BackColor = Color.Transparent;
            chartArea.BorderColor = Color.Red;
            chartArea.AxisX.IsMarginVisible = true;
            chartArea.Area3DStyle.Enable3D = true;

            return chart;
        }
        
    }
}