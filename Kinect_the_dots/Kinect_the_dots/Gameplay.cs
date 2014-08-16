using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kinect_the_dots
{
    public partial class MainWindow
    {
        #region Variables
        /// <summary>
        /// Current connected puzzle index
        /// </summary>
        private int m_puzzleIndex;
        /// <summary>
        /// Tolerance when connecting dots
        /// </summary>
        private double TOLERANCE = 40;
        #endregion

        #region Methods
        /// <summary>
        /// Setup the game
        /// </summary>
        private void SetupGame()
        {
            CreatePuzzle();
        }

        /// <summary>
        /// Create the puzzles
        /// </summary>
        private void CreatePuzzle()
        {
            m_puzzleIndex = -1;
            ReadXMLFile(@".\..\..\Puzzles\Fish.xml");
        }

        /// <summary>
        /// Checks whether player connected all dots
        /// </summary>
        /// <param name="x">Mapped hand x coordinate</param>
        /// <param name="y">Mapped hand y coordinate</param>
        private void IsGameOver(int x, int y)
        {
            if (m_puzzleIndex == m_Points.Count - 1)
                return;

            Point currentPoint;

            if (m_puzzleIndex + 1 < m_Points.Count)
                currentPoint = m_Points[m_puzzleIndex + 1];
            else
                currentPoint = m_Points[0];

            Point distance = new Point(currentPoint.X - x, currentPoint.Y - y);
            double length = Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y));

            ConnectDots(length, currentPoint, x, y);
        }

        private void ConnectDots(double length, Point currentPoint, int x, int y)
        {
            if (length < TOLERANCE)
            {
                if (Crayon.Points.Count - 1 > 0)
                    Crayon.Points.RemoveAt(Crayon.Points.Count - 1);

                Crayon.Points.Add(new Point(currentPoint.X + (ELLIPSE_SIZE / 2), currentPoint.Y + (ELLIPSE_SIZE / 2)));
                Crayon.Points.Add(new Point(currentPoint.X + (ELLIPSE_SIZE / 2), currentPoint.Y + (ELLIPSE_SIZE / 2)));
                m_puzzleIndex++;

                if (m_puzzleIndex == m_Points.Count - 1)
                    Crayon.Points.Add(new Point(m_Points[0].X + (ELLIPSE_SIZE / 2), m_Points[0].Y + (ELLIPSE_SIZE / 2)));
                return;
            }

            if (Crayon.Points.Count - 1 > 0)
            {
                Crayon.Points.RemoveAt(Crayon.Points.Count - 1);
                Crayon.Points.Add(new Point(x, y));
            }
        }
        #endregion
    }
}
