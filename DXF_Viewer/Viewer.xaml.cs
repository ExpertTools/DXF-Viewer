using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.ComponentModel;
using DXF.Viewer.Model;
using DXF.Viewer.Parsing;

namespace DXF.Viewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl, INotifyPropertyChanged
    {
        public double zoomScale = 1;
        private Point origin;
        private Point start;

        public double xOffset { get; set; }
        public double yOffset { get; set; }
        public bool ErrFlag { get; set; }
        public string ErrString { get; set; }
        public List<String> drawOrder = new List<String>();

        private const double STARTING_LINE_THICKNESS = 1.0;
        private const double STARTING_VIEWER_HEIGHT = 300;
        private const double STARTING_VIEWER_WIDTH = 300;

        Schematic drawing;

        public Viewer()
        {
            try
            {
                LineThickness = 2.0 / 96.0 / zoomScale ;
                ErrFlag = false;
                ErrString = "";
                InitializeComponent();
                TransformGroup group = new TransformGroup();
                ScaleTransform xform = new ScaleTransform();
                TranslateTransform tt = new TranslateTransform();
                //Transforms the children on the canvas with scale and translate transform immediately
                group.Children.Add(xform);
                group.Children.Add(tt);
                mainCanvas.RenderTransform = group;
                vbxBorder.MouseWheel += mouseWheel;
                vbxBorder.MouseMove += mouseMove;
                vbxBorder.MouseLeftButtonDown += mouseLeftButtonDown;
                vbxBorder.MouseLeftButtonUp += mouseLeftButtonUp;
                vbxBorder.SizeChanged += sizeChanged;
                PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);
                

            }
            catch (System.Exception ex)
            {
                ErrFlag = true;
                ErrString += "General exception in Viewer.xaml.cs.Viewer: " + ex.Message;
                
            }

        }

        public void DxfStart(string source)
        {
            try
            {
                viewDXF(source);
                //viewDXF_dep(source);
                mainCanvas.Height = Math.Abs(drawing.header.height + 1);
                mainCanvas.Width = Math.Abs(drawing.header.width + 1);
                origin.X = (mainCanvas.Width - 1) / 2;
                origin.Y = (mainCanvas.Height - 1) / 2;

                double viewArea = STARTING_VIEWER_HEIGHT * STARTING_VIEWER_WIDTH;
                double areaRatio = (viewArea / drawing.header.area);
                LineThickness = STARTING_LINE_THICKNESS / zoomScale / Math.Sqrt(areaRatio);
            }
            catch (Exception ex)
            {
                ErrFlag = true;
                Console.WriteLine(ex.StackTrace);
                ErrString += "General exception in Viewer.xaml.cs.DxfStart: " + ex.Message;
            }
        }

        private void viewDXF(string source)
        {
            DXFFile fileRep = new DXFFile(source);
            drawing = new Schematic(fileRep, this, mainCanvas);
            drawing.draw(mainCanvas);
        }

        //**************************
        //this section commented out until zoom-pan is working properly
        // with the mouse.
        //S Damman - 2013-Dec-13
        //**************************
        ////takes keyboard input for zooming in and out of the canvas
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //   var tt = (TranslateTransform)((TransformGroup)mainCanvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            //   //moves focus to the keyboard instead of staying on the button
            //   e.Handled = true;

            //   if (e.Key == Key.Up)
            //   {
            //      e.Handled = true;
            //      tt.Y -= .1;

            //   }
            //   if (e.Key == Key.Down)
            //   {
            //      e.Handled = true;
            //      tt.Y += .1;
            //   }
            //   if (e.Key == Key.Left)
            //   {
            //      e.Handled = true;
            //      tt.X += .1;
            //   }
            //   if (e.Key == Key.Right)
            //   {
            //      e.Handled = true;
            //      tt.X -= .1;
            //   }
            //   //allows either the plus on the numberpad or on the top of the keys to work
            //   else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            //   {
            //      e.Handled = true;
            //      TransformGroup transformGroup = (TransformGroup)mainCanvas.RenderTransform;
            //      ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            //      double zoom = e.Key > 0 ? .2 : -.2;
            //      zoomScale += zoom;
            //      transform.ScaleX += zoom;
            //      transform.ScaleY += zoom;
            //   }
            //   //allows either the minus on the numberpad or on the top of the alpha keys to work
            //   else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            //   {
            //      e.Handled = true;
            //      TransformGroup transformGroup = (TransformGroup)mainCanvas.RenderTransform;
            //      ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            //      double zoom = e.Key > 0 ? .2 : -.2;
            //      zoomScale -= zoom;
            //      if (zoomScale > 0.1)
            //      {
            //         transform.ScaleX -= zoom;
            //         transform.ScaleY -= zoom;
            //      }
            //      else if (zoomScale <= 0.1)
            //      {
            //         zoomScale += zoom;
            //         transform.ScaleX = 0.1;
            //         transform.ScaleY = 0.1;
            //      }
            //   }
            //   else if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            //   {
            //      zoomScale = 1;

            //      TransformGroup transformGroup = (TransformGroup)mainCanvas.RenderTransform;
            //      ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            //      transform.ScaleY = 1;
            //      transform.ScaleX = 1;
            //   }
            //   //else if (Key.RightCtrl)
            //   else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            //   {
            //      double width = Border.ActualWidth;
            //      double height = Border.ActualHeight;
            //      RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            //      DrawingVisual image = new DrawingVisual();
            //      using (DrawingContext drawing = image.RenderOpen())
            //      {
            //         VisualBrush brush = new VisualBrush(mainCanvas);
            //         drawing.DrawRectangle(brush, null, new Rect(new Point(), new Size(width, height)));
            //      }
            //      bmpCopied.Render(image);
            //      Clipboard.SetImage(bmpCopied);
            //   }
            //   else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            //   {
            //      Border.RenderTransformOrigin = new Point(0.5, 0.5);
            //      tt.X = origin.X / width - 0.5;
            //      tt.Y = origin.Y / height - 0.5;
            //   }

        }

        /// <summary>
        /// Zooms in and out of the canvas with the mousewheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseWheel(object sender, MouseWheelEventArgs e)
        {
            //sets the point that is zoomed in to to be the point where the mouse is
            //in screen coordinates, but then taken and divided by the height
            //and width of the canvas to get a number between 0 and 1 which is what
            //wpf uses to get a render transform origin
            Rto = new Point(e.GetPosition(mainCanvas).X / drawing.header.width,
            e.GetPosition(mainCanvas).Y / drawing.header.height);
            //mainCanvas.RenderTransformOrigin = new Point(start.X, start.Y);

            TransformGroup transformGroup = (TransformGroup)mainCanvas.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            //sets zoom equal to the change in the mousewheel either zoom in or out
            double zoom = e.Delta > 0 ? 1.2 : 0.8; //fancy either increases or decreases by 20%
            zoomScale *= zoom;
            if (zoomScale < 0.6)
                zoomScale = 0.6;

            double viewArea = (double)this.Parent.GetValue(Viewbox.ActualHeightProperty) * (double)this.Parent.GetValue(Viewbox.ActualWidthProperty);
            double areaRatio = (viewArea / drawing.header.area);
            

            LineThickness = STARTING_LINE_THICKNESS / zoomScale / Math.Sqrt(areaRatio);

            //changes the scale for both x and y at the same time to keep them even
            transform.ScaleX = zoomScale;
            transform.ScaleY = zoomScale;
        }

        /// <summary>
        /// When the mouse moves during panning it gets the proper information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseMove(object sender, MouseEventArgs e)
        {

            if (!vbxBorder.IsMouseCaptured)
                return;

            //changes the cursor to the hand if the mouse button is down
            vbxBorder.Cursor = Cursors.Hand;

            var tt = (TranslateTransform)((TransformGroup)mainCanvas.RenderTransform).Children.First(tr => tr is TranslateTransform);

            Vector v = start - e.GetPosition(mainCanvas);

            tt.X -= v.X * Math.Abs(zoomScale);
            tt.Y -= v.Y * Math.Abs(zoomScale);
            start = e.GetPosition(mainCanvas);
        }

        DateTime _lastSizeEvent = DateTime.UtcNow;
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            DateTime now = DateTime.UtcNow;
            double areaChanged = Math.Sqrt(Math.Abs(e.NewSize.Height * e.NewSize.Width - e.PreviousSize.Width * e.PreviousSize.Height));
            if(now.Subtract(_lastSizeEvent).TotalSeconds >= 1 && areaChanged > 200)
            {
                double viewArea = (double)this.Parent.GetValue(Viewbox.ActualHeightProperty) * (double)this.Parent.GetValue(Viewbox.ActualWidthProperty);
                double areaRatio = (viewArea / drawing.header.area);
                LineThickness = STARTING_LINE_THICKNESS / zoomScale / Math.Sqrt(areaRatio);
                _lastSizeEvent = now;
            }
        }

        //Releases the mouse capture when the left button is released
        private void mouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            vbxBorder.ReleaseMouseCapture();
            //Once mouse is released then the cursor becomes an arrow again
            vbxBorder.Cursor = Cursors.Arrow;
        }

        //changes where the origin is depending on where the mouse is when the left button is pressed down
        private void mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(mainCanvas);
            vbxBorder.CaptureMouse();
        }

        private void ZoomToFit(object sender, RoutedEventArgs e)
        {
            zoomScale = 1;

            TransformGroup transformGroup = (TransformGroup)mainCanvas.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            transform.ScaleY = 1;
            transform.ScaleX = 1;

            var tt = (TranslateTransform)((TransformGroup)mainCanvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            tt.X = origin.X / drawing.header.width - 0.5;
            tt.Y = origin.Y / drawing.header.height - 0.5;
        }

        private void CopyToClipBoard(object sender, RoutedEventArgs e)
        {
            double width = vbxBorder.ActualWidth;
            double height = vbxBorder.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(drawing.header.width), (int)Math.Round(drawing.header.height), 96, 96, PixelFormats.Default);
            DrawingVisual image = new DrawingVisual();
            using (DrawingContext drawingContext = image.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(mainCanvas);
                drawingContext.DrawRectangle(brush, null, new Rect(new Point(), new Size(drawing.header.width, drawing.header.height)));
            }
            bmpCopied.Render(image);
            Clipboard.SetImage(bmpCopied);
        }


        private double lineThickness = .3;
        public double LineThickness
        {
            get
            {
                return lineThickness;
            }
            set
            {
                if (value != lineThickness)
                {
                    lineThickness = value;
                    NotifyPropertyChanged("LineThickness");
                }
            }
        }

        private Point rto = new Point(0.5, 0.5);
        public Point Rto
        {
            get
            {
                return rto;
            }
            set
            {
                if (value != rto)
                {
                    rto = value;
                    RtoString = rto.X.ToString() + ", " + rto.Y.ToString();
                    NotifyPropertyChanged("Rto");
                }
            }
        }

        private string rtoString = "0.5, 0.5";
        public string RtoString
        {
            get
            {
                return rtoString;
            }
            set
            {
                if (value != rtoString)
                {
                    rtoString = value;
                    NotifyPropertyChanged("RtoString");
                }
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
