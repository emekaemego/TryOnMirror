using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Linq;
using Image = System.Drawing.Image;

namespace SymaCord.TryOnMirror.CV.Impl
{
    public class CvDetection : ICvDetection
    {
        private HaarCascade _haar;
        private double scaleFactor = 1.2;
        private int minNeighbors = 1;

        /// <summary>
        /// Detect facial features; eyes, mouth and eyebrows. This method is intended to be used after a successful facial
        /// detection, which the detected face rect would be passed to the 'faceRect' parameter
        /// </summary>
        /// <param name="image"></param>
        /// <param name="faceRect">Face region of interest for the provided image</param>
        /// <returns></returns>
        public Dictionary<string, Rectangle> DetectfacialFeatures(Bitmap image, Rectangle faceRect)
        {
            var result = new Dictionary<string, Rectangle>();

            var eyesHaar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.EyePair));
            var mouthHaar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.Mouth));

            using (var colorImage = new Image<Rgb, byte>(image))
            {
                //Create a gray scale image from the original image
                var grayImage = colorImage.Convert<Gray, byte>();

                var eyesImage = grayImage.Clone();

                //If the 'faceRect' argument is provided, devide it by 2 and assign the eyes region of interest to the top
                //of the divided rect
                var eyesRoi = !faceRect.IsEmpty
                                  ? new Rectangle(faceRect.X, faceRect.Y, faceRect.Width/2, faceRect.Height/2)
                                  : Rectangle.Empty;

                eyesImage.ROI = eyesRoi;

                var detectedEyes = eyesImage.DetectHaarCascade(eyesHaar, scaleFactor, minNeighbors,
                                                               HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(30, 30))[0
                    ];

                //check if eyepair were detected, if yes, perform some image processing on the image to get the left and right
                //eyes
                if (detectedEyes.Length > 0)
                {
                    //Get the rectangle area of the eyes
                    var detectEyesRect = detectedEyes[0].rect;

                    //Now that we have the location of the eyepair, I'm gonna gonna horizontally devide the rectangle by 2
                    //to get two halfs of each eyes

                    //Starting witht the left eye
                    var leftEyeRoi = new Rectangle(0, 0, detectEyesRect.Width/2, detectEyesRect.Height/2);

                    //Now the right eye area
                    var rightEyeRoi = new Rectangle(leftEyeRoi.Width, 0, detectEyesRect.Width/2,
                                                     detectEyesRect.Height/2);

                    //Create a Bitmap version of the (Image<Gray, byte>) eyesImage above
                    var bitmapEyesImage = eyesImage.ToBitmap();

                    //Create a crop filter
                    var cropFilter = new Crop(leftEyeRoi);

                    //Crop the left eye part out of the eyepair image
                    var leftEyeImage = cropFilter.Apply(bitmapEyesImage);

                    //Crop the left eye part out of the eyepair image
                    cropFilter = new Crop(rightEyeRoi);
                    var rightEyeImage = cropFilter.Apply(bitmapEyesImage);

                    // create filters sequence
                    var filter = new FiltersSequence();
                    filter.Add(new BradleyLocalThresholding());
                    filter.Add(new CannyEdgeDetector());

                    //Start the left eye detection process
                    leftEyeImage = filter.Apply(leftEyeImage);

                    // create an instance of blob counter algorithm
                    BlobCounterBase bc = new BlobCounter();
                    // set filtering options
                    bc.FilterBlobs = true;
                    bc.MinWidth = 10;
                    bc.MinHeight = 10;

                    // set ordering options
                    bc.ObjectsOrder = ObjectsOrder.Size;

                    // process binary image
                    bc.ProcessImage(leftEyeImage);

                    foreach(var blob in bc.GetObjectsInformation())
                    {
                        
                    }
                }
            }

            return result;
        }

        public Rectangle DetectFace(Bitmap image, Rectangle roi)
        {
            var result = Rectangle.Empty;
            _haar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.Face));

                using (var currentFrame = new Image<Gray, byte>(image))
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();
                    grayFrame.ROI = roi;

                    var detectedFeature =
                        grayFrame.DetectHaarCascade(_haar, scaleFactor, minNeighbors, 0, new Size(30, 30))[0];

                    //foreach (var face in detectedFaces)
                    if (detectedFeature.Length > 0)
                        result = detectedFeature[0].rect;
                }

            return result;
        }

        public Rectangle DetectFace(Bitmap image)
        {
            return DetectFace(image, Rectangle.Empty);
        }

        public Rectangle DetectFace(string imageFilePath)
        {
            var image = (Bitmap) Image.FromFile(imageFilePath);
            var result = DetectFace(image);

            return result;
        }

        public Rectangle DetectLeftEye(Bitmap image, Rectangle roi)
        {
            var result = Rectangle.Empty;

            using (_haar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.LeftEye)))
            {
                using (var currentFrame = new Image<Bgr, byte>(image))
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();
                    
                    //set image region of interest
                    grayFrame.ROI = roi;

                    var detectedFeature =
                        grayFrame.DetectHaarCascade(_haar, scaleFactor, minNeighbors, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                                    new Size(40, 40))[0];

                    //Get only the first detected part
                    //foreach (var face in detectedFaces)
                    if (detectedFeature.Length > 0)
                    {
                        //Crop out the detected eye from the image
                        var rect = detectedFeature[0].rect;
                        var crop = new Crop(rect);
                        var eyeImage = crop.Apply(grayFrame.ToBitmap());

                        var blobs = DetectBlobs(eyeImage, rect.Width / 4, rect.Height / 4);

                        //If the blobs count is above one, then my guess is that the eyebrow is part of it. What I did below
                        //is to get any of the blob that has the highest amount of Y coordinate as the eye. But, if we have
                        //only 1 blob, then return it as the eye
                        if(blobs.Length >= 2)
                        {
                            var blob = blobs.OrderByDescending(x => x.Rectangle.Y).FirstOrDefault();

                            if (blob != null)
                                result = blob.Rectangle;
                        }else if(blobs.Length == 1)
                        {
                            result = blobs[0].Rectangle;
                        }

                        result.X += rect.X;
                        result.Y += rect.Y;
                    }
                    //Well, if we make it down here, it means that Emgu CV failed me, am gonna do the brave thing 
                    //and try detecting the feature myself. Hit it. 
                    else
                    {
                        var eyeImage = grayFrame.ToBitmap();

                        var blobs = DetectBlobs(eyeImage, eyeImage.Width / 4, eyeImage.Height / 4);

                        //If the blobs count is above one, then my guess is that the eyebrow is part of it. What I did below
                        //is to get any of the blob that has the highest amount of Y coordinate as the eye. But, if we have
                        //only 1 blob, then return it as the eye
                        if (blobs.Length >= 2)
                        {
                            var blob = blobs.OrderByDescending(x => x.Rectangle.Y).FirstOrDefault();

                            if (blob != null)
                                result = blob.Rectangle;
                        }
                        else if (blobs.Length == 1)
                        {
                            result = blobs[0].Rectangle;
                        }
                    }
                }
            }

            if (!roi.IsEmpty)
            {
                result.X += roi.X;
                result.Y += roi.Y;
            }

            return result;
        }

        public Rectangle DetectLeftEye(Bitmap image)
        {
            return DetectLeftEye(image, Rectangle.Empty);
        }

        public Rectangle DetectLeftEye(string imageFilePath)
        {
            var image = (Bitmap)Image.FromFile(imageFilePath);
            var result = DetectLeftEye(image);

            return result;
        }

        public Rectangle DetectRightEye(Bitmap image, Rectangle roi)
        {
            var result = Rectangle.Empty;

            using (_haar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.RightEye)))
            {
                using (var currentFrame = new Image<Bgr, byte>(image))
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();

                    //set image region of interest
                    grayFrame.ROI = roi;

                    var detectedFeature =
                        grayFrame.DetectHaarCascade(_haar, scaleFactor, minNeighbors, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                                    new Size(40, 40))[0];

                    //Get only the first detected part
                    //foreach (var face in detectedFaces)
                    if (detectedFeature.Length > 0)
                    {
                        //Crop out the detected eye from the image
                        var rect = detectedFeature[0].rect;
                        var crop = new Crop(rect);
                        var eyeImage = crop.Apply(grayFrame.ToBitmap());

                        var blobs = DetectBlobs(eyeImage, rect.Width / 4, rect.Height / 4);

                        //If the blobs count is above one, then my guess is that the eyebrow is part of it. What I did below
                        //is to get any of the blob that has the highest amount of Y coordinate as the eye. But, if we have
                        //only 1 blob, then return it as the eye
                        if (blobs.Length >= 2)
                        {
                            var blob = blobs.OrderByDescending(x => x.Rectangle.Y).FirstOrDefault();

                            if (blob != null)
                                result = blob.Rectangle;
                        }
                        else if (blobs.Length == 1)
                        {
                            result = blobs[0].Rectangle;
                        }

                        result.X += rect.X;
                        result.Y += rect.Y;
                    }
                    //Well, if we make it down here, it means that Emgu CV failed me, am gonna do the brave thing 
                    //and try detecting the feature myself. Hit it. 
                    else
                    {
                        var eyeImage = grayFrame.ToBitmap();

                        var blobs = DetectBlobs(eyeImage, eyeImage.Width / 4, eyeImage.Height / 4);

                        //If the blobs count is above one, then my guess is that the eyebrow is part of it. What I did below
                        //is to get any of the blob that has the highest amount of Y coordinate as the eye. But, if we have
                        //only 1 blob, then return it as the eye
                        if (blobs.Length >= 2)
                        {
                            var blob = blobs.OrderByDescending(x => x.Rectangle.Y).FirstOrDefault();

                            if (blob != null)
                                result = blob.Rectangle;
                        }
                        else if (blobs.Length == 1)
                        {
                            result = blobs[0].Rectangle;
                        }
                    }
                }
            }

            if (!roi.IsEmpty)
            {
                result.X += roi.X;
                result.Y += roi.Y;
            }

            return result;
        }

        public Rectangle DetectRightEye(Bitmap image)
        {
            return DetectRightEye(image, Rectangle.Empty);
        }

        public Rectangle DetectRightEye(string imageFilePath)
        {
            var image = (Bitmap)Image.FromFile(imageFilePath);
            var result = DetectRightEye(image);

            return result;
        }

        public Rectangle DetectNose(Bitmap image, Rectangle roi)
        {
            var result = Rectangle.Empty;

            using (_haar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.Nose)))
            {
                using (var currentFrame = new Image<Bgr, byte>(image))
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();

                    grayFrame.ROI = roi;

                    var detectedFeature =
                        grayFrame.DetectHaarCascade(_haar, scaleFactor, minNeighbors, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                                    new Size(currentFrame.Width / 8, currentFrame.Height / 8))[0];

                    //Get only the first detected part
                    //foreach (var face in detectedFaces)
                    if (detectedFeature.Length > 0)
                        result = detectedFeature[0].rect;
                }
            }

            return result;
        }

        public Rectangle DetectNose(Bitmap image)
        {
            return DetectNose(image, Rectangle.Empty);
        }

        public Rectangle DetectNose(string imageFilePath)
        {
            var image = (Bitmap)Image.FromFile(imageFilePath);
            var result = DetectNose(image);

            return result;
        }

        public Rectangle DetectMouth(Bitmap image, Rectangle roi)
        {
            var result = Rectangle.Empty;

            using (_haar = new HaarCascade(GetHaarCascadeFilePath(HaarCascadeType.Mouth)))
            {
                using (var currentFrame = new Image<Bgr, byte>(image))
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();

                    //set image region of interest
                    grayFrame.ROI = roi;

                    var detectedFeature =
                        grayFrame.DetectHaarCascade(_haar, scaleFactor, minNeighbors, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                                    new Size(40, 40))[0];

                    //Get only the first detected part
                    //foreach (var face in detectedFaces)
                    if (detectedFeature.Length > 0)
                    {
                        //Crop out the detected feature from the image
                        var rect = detectedFeature[0].rect;
                        var crop = new Crop(rect);
                        var featureImage = crop.Apply(grayFrame.ToBitmap());

                        var blobs = DetectBlobs(featureImage, rect.Width / 4, rect.Height / 4);

                        //Detected blobs are organized by size, so if there's any detected blob, get the first blob
                        //which is the biggest
                        if (blobs.Length > 0)
                        {
                            result = blobs[0].Rectangle;
                        }

                        result.X += rect.X;
                        result.Y += rect.Y;
                    }
                    //Well, if we make it down here, it means that Emgu CV failed me, am gonna do the brave thing 
                    //and try detecting the feature myself. Hit it. 
                    else
                    {
                        var featureImage = grayFrame.ToBitmap();

                        var blobs = DetectBlobs(featureImage, featureImage.Width / 4, featureImage.Height / 4);

                        //Detected blobs are organized by size, so if there's any detected blob, get the first blob
                        //which is the biggest
                        if (blobs.Length > 0)
                        {
                            result = blobs[0].Rectangle;
                        }
                    }
                }
            }

            if (!roi.IsEmpty)
            {
                result.X += roi.X;
                result.Y += roi.Y;
            }

            return result;
        }

        public Rectangle DetectMouth(Bitmap image)
        {
            return DetectMouth(image, Rectangle.Empty);
        }

        public Rectangle DetectMouth(string imageFilePath)
        {
            var image = (Bitmap)Image.FromFile(imageFilePath);
            var result = DetectMouth(image);

            return result;
        }

        private Blob[] DetectBlobs(Bitmap image, int minWidth, int minHeight)
        {
            // create filters sequence
            var filter = new FiltersSequence();
            filter.Add(new BradleyLocalThresholding());
            filter.Add(new CannyEdgeDetector());

            //Start the left eye detection process
            image = filter.Apply(image);

            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter();
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = minWidth;
            bc.MinHeight = minHeight;

            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;

            // process binary image
            bc.ProcessImage(image);

            return bc.GetObjectsInformation();
        }
        
        //private string getfaceHaarCascadePath()
        //{
        //    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\frontalface_haarcascade.xml";

        //    return path;
        //}

        private string GetHaarCascadeFilePath(HaarCascadeType type)
        {
            string fileName = string.Empty;

            switch(type)
            {
                case HaarCascadeType.Face:
                    fileName = "frontalface_haarcascade.xml";
                    break;
                    case HaarCascadeType.LeftEye:
                    fileName = "haarcascade_mcs_lefteye.xml";
                    break;
                    case HaarCascadeType.RightEye:
                    fileName = "haarcascade_righteye_2splits.xml";
                    break;
                    case HaarCascadeType.Nose:
                    fileName = "haarcascade_mcs_nose.xml";
                    break;
                    case  HaarCascadeType.Mouth:
                    fileName = "haarcascade_mcs_mouth.xml";
                    break;
                    case HaarCascadeType.EyePair:
                    fileName = "haarcascade_mcs_eyepair_big";
                    break;
            }

            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
           var path = Path.Combine(basePath, fileName);

            //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + fileName;

            return path;
        }
    }

    public enum HaarCascadeType
    {
        Face,
        LeftEye,
        RightEye,
        Nose,
        Mouth,
        EyePair
    }
}