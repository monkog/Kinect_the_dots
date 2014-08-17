using Microsoft.Kinect;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_the_dots
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        /// <summary>
        /// Current KinectSensor
        /// </summary>
        private KinectSensor m_kinectSensor;
        /// <summary>
        /// WritableBitmap that source from Kinect camera is written to
        /// </summary>
        private WriteableBitmap m_cameraSourceBitmap;
        /// <summary>
        /// Bounds of camera source
        /// </summary>
        private Int32Rect m_cameraSourceBounds;
        /// <summary>
        /// Number of bytes per line
        /// </summary>
        private int m_colorStride;
        #endregion

        #region Properties
        /// <summary>
        /// Current KinectSensor
        /// </summary>
        public KinectSensor Kinect
        {
            get { return m_kinectSensor; }
            set
            {
                if (m_kinectSensor != value)
                {
                    if (m_kinectSensor != null)
                    {
                        UninitializeKinectSensor(m_kinectSensor);
                        m_kinectSensor = null;
                    }
                    if (value != null && value.Status == KinectStatus.Connected)
                    {
                        m_kinectSensor = value;
                        InitializeKinectSensor(m_kinectSensor);
                    }
                }
            }
        }
        #endregion

        #region Methods
        public MainWindow()
        {
            InitializeComponent();
            Loaded += DiscoverKinectSensors;
            Unloaded += (sender, e) => { Kinect = null; };
            SetupGame();
        }

        /// <summary>
        /// Enables ColorStream from newly detected KinectSensor and sets output image
        /// </summary>
        /// <param name="sensor">Detected KinectSensor</param>
        private void InitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                ColorImageStream colorStream = sensor.ColorStream;
                colorStream.Enable();

                m_cameraSourceBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight
                    , 96, 96, PixelFormats.Bgr32, null);
                m_cameraSourceBounds = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                m_colorStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                KinectCameraImage.Source = m_cameraSourceBitmap;

                sensor.ColorFrameReady += KinectSensor_ColorFrameReady;

                sensor.SkeletonStream.Enable();
                m_skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                sensor.SkeletonFrameReady += KinectSensor_SkeletonFrameReady;
                sensor.Start();
            }
        }

        /// <summary>
        /// Disables ColorStream from disconnected KinectSensor
        /// </summary>
        /// <param name="sensor">Disconnected KinectSensor</param>
        private void UninitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.ColorFrameReady -= KinectSensor_ColorFrameReady;
                sensor.SkeletonFrameReady -= KinectSensor_SkeletonFrameReady;
                sensor.SkeletonStream.Disable();
                m_skeletons = null;
            }
        }

        /// <summary>
        /// Handles ColorFrameReady event
        /// </summary>
        /// <remarks>
        /// Views image from the camera in KinectCameraImage
        /// </remarks>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments containing ImageFrame</param>
        private void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(pixels);

                    m_cameraSourceBitmap.WritePixels(m_cameraSourceBounds, pixels, m_colorStride, 0);
                }
            }
        }

        /// <summary>
        /// Gets the primary skeleton
        /// </summary>
        /// <param name="m_skeletons">Array of discovered skeletons</param>
        /// <returns>Primary skeleton</returns>
        private Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                // Find the skeleton closest to Kinect
                for (int i = 0; i < skeletons.Length; i++)
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                            skeleton = skeletons[i];
                        else if (skeleton.Position.Z > skeletons[i].Position.Z)
                            skeleton = skeletons[i];
                    }
            }

            return skeleton;
        }

        /// <summary>
        /// Gets the primary hand
        /// </summary>
        /// <param name="skeleton">Primary skeleton</param>
        /// <returns>Hand closest to Kinect</returns>
        private Joint GetPrimaryHand(Skeleton skeleton)
        {
            Joint hand = new Joint();

            if (skeleton != null)
            {
                hand = skeleton.Joints[JointType.HandLeft];
                Joint rightHand = skeleton.Joints[JointType.HandRight];

                if (rightHand.TrackingState != JointTrackingState.NotTracked)
                    if (hand.TrackingState == JointTrackingState.NotTracked
                        || hand.Position.Z > rightHand.Position.Z)
                        hand = rightHand;
            }

            return hand;
        }

        /// <summary>
        /// Keeps track of the hand
        /// </summary>
        /// <param name="hand">Primary hand</param>
        private void TrackHand(Joint hand)
        {
            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                HandCursor.Visibility = Visibility.Collapsed;
                return;
            }

            HandCursor.Visibility = Visibility.Visible;

            DepthImagePoint point = Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position
                , Kinect.DepthStream.Format);
            int x = (int)((point.X * HandCanvas.Width / Kinect.DepthStream.FrameWidth));
            int y = (int)((point.Y * HandCanvas.ActualHeight / Kinect.DepthStream.FrameHeight));

            Canvas.SetLeft(HandCursor, x - (HandCursor.ActualWidth / 2.0));
            Canvas.SetTop(HandCursor, y - (HandCursor.Height / 2.0));

            if (hand.JointType == JointType.HandRight)
                CursorScale.ScaleX = 1;
            else
                CursorScale.ScaleX = -1;

            IsGameOver(x, y);
        }

        /// <summary>
        /// Subscribes for StatusChanged event and initializes KinectSensor
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void DiscoverKinectSensors(object sender, RoutedEventArgs e)
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensor_StatusChanged;
            Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }

        /// <summary>
        /// Updates KinectSensor
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void KinectSensor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                case KinectStatus.Connected:
                    if (Kinect == null)
                        Kinect = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    if (Kinect == e.Sensor)
                    {
                        Kinect = null;
                        Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (Kinect == null)
                            MessageBox.Show("No Kinect is connected. Please provide one.");
                    }
                    break;
                default:
                    MessageBox.Show("No Kinect is connected. Please provide one.");
                    break;
            }
        }
        #endregion
    }
}
