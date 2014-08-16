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
        private List<Point> m_Points;
        /// <summary>
        /// Name of root node
        /// </summary>
        private const string ROOT_NODE = "Puzzle";
        /// <summary>
        /// Name of image node
        /// </summary>
        private const string IMAGE_NODE = "Image";
        /// <summary>
        /// Name of point node
        /// </summary>
        private const string POINT = "Point";
        /// <summary>
        /// Name of X coordinate node
        /// </summary>
        private const string X_COORD = "X";
        /// <summary>
        /// Name of Y coordinate node
        /// </summary>
        private const string Y_COORD = "Y";
        /// <summary>
        /// Ellipse size
        /// </summary>
        private const int ELLIPSE_SIZE = 50;
        /// <summary>
        /// Ellipse stroke size
        /// </summary>
        private const int ELLIPSE_STROKE_SIZE = 2;
        /// <summary>
        /// Ellipse color
        /// </summary>
        private Color ELLIPSE_COLOR = Colors.SteelBlue;
        /// <summary>
        /// Ellipse stroke color
        /// </summary>
        private Color ELLIPSE_STROKE_COLOR = Colors.Black;
        /// <summary>
        /// Label font color
        /// </summary>
        private Color FONT_COLOR = Colors.Black;
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
        private void ReadXMLFile(string file)
        {
            m_Points = new List<Point>();
            DotCanvas.Children.Clear();

            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode background = document.SelectSingleNode(ROOT_NODE).SelectSingleNode(IMAGE_NODE);
            // TODO: Read background

            XmlNodeList points = document.SelectSingleNode(ROOT_NODE).SelectNodes(POINT);
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
                int x = int.Parse(point.SelectSingleNode(X_COORD).InnerText);
                int y = int.Parse(point.SelectSingleNode(Y_COORD).InnerText);
                m_Points.Add(new Point(x, y));

                Grid dot = new Grid();
                dot.Children.Add(CreateEllipse());
                dot.Children.Add(CreateLabel(m_Points.Count));

                Canvas.SetLeft(dot, x);
                Canvas.SetTop(dot, y);
                DotCanvas.Children.Add(dot);
            }
        }

        /// <summary>
        /// Creates a single ellipse
        /// </summary>
        /// <returns>New Ellipse instance</returns>
        private Ellipse CreateEllipse()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = ELLIPSE_SIZE;
            ellipse.Height = ELLIPSE_SIZE;
            ellipse.Fill = new SolidColorBrush(ELLIPSE_COLOR);
            ellipse.StrokeThickness = ELLIPSE_STROKE_SIZE;
            ellipse.Stroke = new SolidColorBrush(ELLIPSE_STROKE_COLOR);

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
            label.Foreground = new SolidColorBrush(FONT_COLOR);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;

            return label;
        }
        #endregion
    }
}
