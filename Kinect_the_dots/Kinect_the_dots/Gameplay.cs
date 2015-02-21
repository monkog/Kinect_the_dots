using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// Path to the xml puzzle files
        /// </summary>
        private const string DirectoryPath = @".\..\..\Puzzles";
        /// <summary>
        /// Current puzzle number
        /// </summary>
        private int _currentPuzzle;
        /// <summary>
        /// List of puzzle files
        /// </summary>
        private List<string> _puzzles;
        /// <summary>
        /// Current connected puzzle index
        /// </summary>
        private int _puzzleIndex;
        /// <summary>
        /// Tolerance when connecting dots
        /// </summary>
        private const double Tolerance = 40;
        #endregion

        #region Methods
        /// <summary>
        /// Setup the game
        /// </summary>
        private void SetupGame()
        {
            _currentPuzzle = 0;
            ReadPuzzleFiles();
            CreatePuzzle();
        }

        /// <summary>
        /// Finds paths to all puzzle files
        /// </summary>
        private void ReadPuzzleFiles()
        {
            _puzzles = new List<string>();

            foreach (string file in Directory.GetFiles(DirectoryPath))
                if (Path.GetExtension(file) == ".xml")
                    _puzzles.Add(file);
        }

        /// <summary>
        /// Create the puzzles
        /// </summary>
        private void CreatePuzzle()
        {
            _puzzleIndex = -1;
            ReadXmlFile(_puzzles[_currentPuzzle]);
        }

        /// <summary>
        /// Checks whether player connected all dots
        /// </summary>
        /// <param name="x">Mapped hand x coordinate</param>
        /// <param name="y">Mapped hand y coordinate</param>
        private void IsGameOver(int x, int y)
        {
            if (_puzzleIndex == _points.Count - 1)
                return;

            Point currentPoint;

            if (_puzzleIndex + 1 < _points.Count)
                currentPoint = _points[_puzzleIndex + 1];
            else
                currentPoint = _points[0];

            Point distance = new Point(currentPoint.X - x, currentPoint.Y - y);
            double length = Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y));

            ConnectDots(length, currentPoint, x, y);
        }

        private void ConnectDots(double length, Point currentPoint, int x, int y)
        {
            if (length < Tolerance)
            {
                if (Crayon.Points.Count - 1 > 0)
                    Crayon.Points.RemoveAt(Crayon.Points.Count - 1);

                Crayon.Points.Add(new Point(currentPoint.X + (EllipseSize / 2), currentPoint.Y + (EllipseSize / 2)));
                Crayon.Points.Add(new Point(currentPoint.X + (EllipseSize / 2), currentPoint.Y + (EllipseSize / 2)));
                _puzzleIndex++;

                if (_puzzleIndex == _points.Count - 1)
                    Crayon.Points.Add(new Point(_points[0].X + (EllipseSize / 2), _points[0].Y + (EllipseSize / 2)));
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
