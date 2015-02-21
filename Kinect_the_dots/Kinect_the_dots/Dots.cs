using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace Kinect_the_dots
{
    public partial class MainWindow
    {
        #region Variables
        /// <summary>
        /// List of points in current puzzle
        /// </summary>
        private List<Point> _points;
        /// <summary>
        /// Name of root node
        /// </summary>
        private const string RootNode = "Puzzle";
        /// <summary>
        /// Name of image node
        /// </summary>
        private const string ImageNode = "Image";
        /// <summary>
        /// Name of point node
        /// </summary>
        private const string PointNode = "Point";
        /// <summary>
        /// Name of X coordinate node
        /// </summary>
        private const string XCoord = "X";
        /// <summary>
        /// Name of Y coordinate node
        /// </summary>
        private const string YCoord = "Y";
        /// <summary>
        /// Ellipse size
        /// </summary>
        private const int EllipseSize = 50;
        /// <summary>
        /// Ellipse stroke size
        /// </summary>
        private const int EllipseStrokeSize = 2;
        /// <summary>
        /// Ellipse color
        /// </summary>
        private Color _ellipseColor = Colors.SteelBlue;
        /// <summary>
        /// Ellipse stroke color
        /// </summary>
        private Color _ellipseStrokeColor = Colors.Black;
        /// <summary>
        /// Label font color
        /// </summary>
        private Color _fontColor = Colors.Black;
        /// <summary>
        /// Label font size
        /// </summary>
        private const double FONT_SIZE = 24;
        #endregion

        #region Methods
        /// <summary>
        /// Reads the XML puzzle file and sets the background and places the dots
        /// </summary>
        /// <param name="file">File to read puzzle from</param>
        private void ReadXmlFile(string file)
        {
            _points = new List<Point>();
            DotCanvas.Children.Clear();

            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode background = document.SelectSingleNode(RootNode).SelectSingleNode(ImageNode);
            // TODO: Read background

            XmlNodeList points = document.SelectSingleNode(RootNode).SelectNodes(PointNode);
            PlaceDots(points);
        }

        /// <summary>
        /// Places the dots on the screen
        /// </summary>
        /// <param name="points">List of XML Points</param>
        private void PlaceDots(XmlNodeList points)
        {
            foreach (XmlNode point in points)
            {
                int x = int.Parse(point.SelectSingleNode(XCoord).InnerText);
                int y = int.Parse(point.SelectSingleNode(YCoord).InnerText);
                _points.Add(new Point(x, y));

                Grid dot = new Grid();
                dot.Children.Add(CreateEllipse());
                dot.Children.Add(CreateLabel(_points.Count));

                Canvas.SetLeft(dot, x);
                Canvas.SetTop(dot, y);
                DotCanvas.Children.Add(dot);
            }
            //DotCanvas.Children.Add(DotCanvas.Children[0]);

            _points.Add(new Point(Canvas.GetLeft(DotCanvas.Children[0]), Canvas.GetTop(DotCanvas.Children[0])));
        }

        /// <summary>
        /// Creates a single ellipse
        /// </summary>
        /// <returns>New Ellipse instance</returns>
        private Ellipse CreateEllipse()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = EllipseSize;
            ellipse.Height = EllipseSize;
            ellipse.Fill = new SolidColorBrush(_ellipseColor);
            ellipse.StrokeThickness = EllipseStrokeSize;
            ellipse.Stroke = new SolidColorBrush(_ellipseStrokeColor);

            return ellipse;
        }

        /// <summary>
        /// Creates a Label with prober order number
        /// </summary>
        /// <param name="number">Number in order</param>
        /// <returns>TextBlock with a proper number</returns>
        private TextBlock CreateLabel(int number)
        {
            TextBlock label = new TextBlock();
            label.Text = number.ToString();
            label.FontSize = FONT_SIZE;
            label.Foreground = new SolidColorBrush(_fontColor);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;

            return label;
        }
        #endregion
    }
}
