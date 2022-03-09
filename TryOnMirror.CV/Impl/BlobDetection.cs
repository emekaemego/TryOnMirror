using System.Collections.Generic;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using Emgu.CV;
using Emgu.CV.Structure;
using Image = System.Drawing.Image;

namespace SymaCord.TryOnMirror.CV.Impl
{
    public class BlobDetection : IBlobDetection
    {
        //Aforge,Net implementation
        public Rectangle FindBiggestBlob(Bitmap originalImage, Rectangle roi, out System.Drawing.Point[] points)
        {
            var result = Rectangle.Empty;
            points = new System.Drawing.Point[] {};

            // create filters sequence
            var filter = new FiltersSequence();

            // add filters to the sequence
            filter.Add(new Grayscale(0.299, 0.587, 0.114));
            filter.Add(new BradleyLocalThresholding());
            //filter.Add(new HomogenityEdgeDetector());
            //filter.Add(new DifferenceEdgeDetector());
            filter.Add(new CannyEdgeDetector());

            // apply the filter sequence
            Bitmap image = filter.Apply(originalImage);

            //create region of interest while still preserve original size of image and roi on the same 
            //location/coordinate on the original image

            //Create a new Emgu CV gray image with region of interest specified
            var grayImage = new Image<Gray, byte>(image) {ROI = roi};

            //Create a blank image with same size as the original image
            var blankImage = new Bitmap(originalImage.Width, originalImage.Height /*, PixelFormat.Format24bppRgb*/);

            //Create a Graphics object from the blank image
            var g = Graphics.FromImage(blankImage);

            //Draw the region of inerest image untop of the blank image
            g.DrawImage(grayImage.ToBitmap(), roi.X, roi.Y, roi.Width, roi.Height);

            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter();
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = 5;
            bc.MinHeight = 5;

            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;
            // process binary image
            bc.ProcessImage(blankImage);

            Blob[] blobs = bc.GetObjectsInformation();
            // extract the biggest blob
            if (blobs.Length > 0)
            {
                bc.ExtractBlobsImage(blankImage, blobs[0], true);

                // create convex hull searching algorithm
                GrahamConvexHull hullFinder = new GrahamConvexHull();

                /*
                // lock image to draw on it
                BitmapData data = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                                                         ImageLockMode.ReadWrite, originalImage.PixelFormat);
                */

                List<IntPoint> leftPoints, rightPoints, edgePoints = new List<IntPoint>();

                // get blob's edge points
                bc.GetBlobsLeftAndRightEdges(blobs[0], out leftPoints, out rightPoints);

                edgePoints.AddRange(leftPoints);
                edgePoints.AddRange(rightPoints);

                // blob's convex hull
                List<IntPoint> hull = hullFinder.FindHull(edgePoints);
                points = ToPointsArray(bc.GetBlobsEdgePoints(blobs[0]));

                result = bc.GetObjectsRectangles()[0];
                //Drawing.Polygon(data, hull, Color.Red);

                //originalImage.UnlockBits(data);

                //Dispose
                g.Dispose();
                image.Dispose();
                grayImage.Dispose();
                //blankImage.Dispose();
            }

            return result;
        }

        public Rectangle FindBiggestBlob(string imageFillPath, Rectangle roi, out System.Drawing.Point[] points )
        {
            var originalImage = (Bitmap)Image.FromFile(imageFillPath);

            var result = this.FindBiggestBlob(originalImage, roi, out points);

            originalImage.Dispose();

            return result;
        }

        // Conver list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            var array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
    }
}
