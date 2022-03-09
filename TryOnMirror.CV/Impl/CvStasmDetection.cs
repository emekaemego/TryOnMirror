using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Entities;

//using Point = SymaCord.VirtualMakeover.Entities.Point;

namespace SymaCord.TryOnMirror.CV.Impl
{
    public class CvStasmDetection : ICvStasmDetection
    {
        [DllImport(@"stasm\stasm_dll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError=true)]
        internal static extern void AsmSearchDll
            (
            [Out] out Int32 pnlandmarks,
            [Out] Int32[] landmarks,
            [In, MarshalAs(UnmanagedType.LPStr)] String image_name,
            [In] IntPtr image_data,
            [In] Int32 width,
            [In] Int32 height,
            [In] Int32 is_color,
            [In, MarshalAs(UnmanagedType.LPStr)] String conf_file0,
            [In, MarshalAs(UnmanagedType.LPStr)] String conf_file1);

        //public List<CvSearchResult> SearchFacialFeatures(string imagePath, Rectangle roi)
        //{
        //    return SearchFacialFeatures(new Bitmap(imagePath), imagePath, roi);
        //}

        public Dictionary<string, CvResult> SearchFacialFeatures(string imagePath)
        {
            return SearchFacialFeatures(imagePath, Rectangle.Empty);
        }

        //public List<CvSearchResult> SearchFacialFeatures(Image image, string imagePath)
        //{
        //    return SearchFacialFeatures(image, imagePath, Rectangle.Empty);
        //}

        public Dictionary<string, CvResult> SearchFacialFeatures(string imagePath, Rectangle roi)
        {
            var result = new Dictionary<string, CvResult>();

            //var bgrImage = new Image<Bgr, byte>(imagePath);

            var appDomain = AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            //var path = Path.Combine(basePath, fileName);

            var img = new Image<Bgr, byte>(imagePath);
            //img._EqualizeHist();

            string data_dir = Path.Combine(basePath, @"data");
            int pnlandmarks = 0;
            var landmarks = new int[200];
            var image_name = imagePath;
            var imageData = img.MIplImage.imageData;
            int imgWidth = img.Width;
            int imgHeight = img.Height;
            int is_color = 1;
            string confile_file0 = Path.Combine(basePath, @"stasm\data\mu-68-1d.conf"); //Path.GetFullPath(@"..\data\mu-68-1d.conf");
            string config_file1 = Path.Combine(basePath, @"stasm\data\mu-76-2d.conf"); //Path.GetFullPath(@"..\data\mu-76-2d.conf");

            if (img.Width % 4 != 0)
            {
                throw new InvalidDataException("Invalid image width");
            }
            
            AsmSearchDll(out pnlandmarks, landmarks, image_name, imageData, imgWidth, imgHeight, is_color,
                         confile_file0, config_file1);

            //Check if any feature is found, and add their landmarks to the coords variable
            if (pnlandmarks > 0)
            {
                //Get all landmarks that doesn't have zero 0 as value
                var validMarks = landmarks.Where(x => x > 0).ToArray();

                var coords = validMarks.ToCoordsArray();

                //Create landmark coords for each facial features
                
                //Face landmark
                var faceCoords = new Coord[4];
                faceCoords[0] = new Coord(coords[0].X, coords[0].Y);
                faceCoords[1] = new Coord(coords[7].X, coords[7].Y);
                faceCoords[2] = new Coord(coords[14].X, coords[14].Y);
                faceCoords[3] = new Coord(coords[7].X, 
                    coords[0].Y - ((coords[41].Y - coords[0].Y) + (coords[61].Y - coords[41].Y))); //Hair line
                //faceCoords[3] = new Coord(coords[7].X, coords[7].Y);
                //faceCoords[4] = new Coord(coords[11].X, coords[11].Y);
                //faceCoords[5] = new Coord(coords[13].X, coords[13].Y);
                //faceCoords[6] = new Coord(coords[14].X, coords[14].Y);
                
                //Left eye landmark
                var leftEyeCoords = new Coord[4];
                leftEyeCoords[0] = new Coord(coords[27].X, coords[27].Y);
                leftEyeCoords[1] = new Coord(coords[28].X, coords[28].Y);
                leftEyeCoords[2] = new Coord(coords[29].X, coords[29].Y);
                leftEyeCoords[3] = new Coord(coords[30].X, coords[30].Y);

                //Right eye landmark
                var rightEyeCoords = new Coord[4];
                rightEyeCoords[0] = new Coord(coords[34].X, coords[34].Y);
                rightEyeCoords[1] = new Coord(coords[33].X, coords[33].Y);
                rightEyeCoords[2] = new Coord(coords[32].X, coords[32].Y);
                rightEyeCoords[3] = new Coord(coords[35].X, coords[35].Y);

                var leftEyePupilCoord = new Coord(coords[31].X, coords[31].Y);
                var leftEyeballRadius = (coords[69].X - coords[68].X) / 2;
                //leftEyeballCoords[0] = new Coord(coords[68].X, coords[68].Y); //iris start coord
                //leftEyeballCoords[1] = new Coord(coords[31].X, coords[31].Y); //iris center coord (pupil)
                //leftEyeballCoords[2] = new Coord(coords[69].X, coords[69].Y); //iris end coord

                var rightEyePupilCoord = new Coord(coords[36].X, coords[36].Y);
                var rightEyeballRadius = (coords[72].X - coords[73].X) / 2;
                //rightEyeballCoords[0] = new Coord(coords[73].X, coords[73].Y); //iris start coord
                //rightEyeballCoords[1] = new Coord(coords[36].X, coords[36].Y); //iris center coord (pupil)
                //rightEyeballCoords[2] = new Coord(coords[72].X, coords[72].Y); //iris end coord

                var leftEyeBrowCoords = new Coord[6];
                leftEyeBrowCoords[0] = new Coord(coords[21].X, coords[21].Y);
                leftEyeBrowCoords[1] = new Coord(coords[22].X, coords[22].Y);
                leftEyeBrowCoords[2] = new Coord(coords[23].X, coords[23].Y);
                leftEyeBrowCoords[3] = new Coord(coords[24].X, coords[24].Y);
                leftEyeBrowCoords[4] = new Coord(coords[25].X, coords[25].Y);
                leftEyeBrowCoords[5] = new Coord(coords[26].X, coords[26].Y);

                var rightEyeBrowCoords = new Coord[6];
                rightEyeBrowCoords[0] = new Coord(coords[18].X, coords[18].Y);
                rightEyeBrowCoords[1] = new Coord(coords[17].X, coords[17].Y);
                rightEyeBrowCoords[2] = new Coord(coords[16].X, coords[16].Y);
                rightEyeBrowCoords[3] = new Coord(coords[15].X, coords[15].Y);
                rightEyeBrowCoords[4] = new Coord(coords[20].X, coords[20].Y);
                rightEyeBrowCoords[5] = new Coord(coords[19].X, coords[19].Y);

                var noseCoords = new Coord[1];
                //noseLineCoords[0] = new Coord(coords[40].X, coords[40].Y);
                noseCoords[0] = new Coord(coords[41].X, coords[41].Y); //nose center tip
                //noseLineCoords[2] = new Coord(coords[42].X, coords[42].Y);

                var lipsCoords = new Coord[8];
                lipsCoords[0] = new Coord(coords[48].X, coords[48].Y);
                lipsCoords[1] = new Coord(coords[50].X, coords[50].Y);
                lipsCoords[2] = new Coord(coords[51].X, coords[51].Y);
                lipsCoords[3] = new Coord(coords[52].X, coords[52].Y);
                lipsCoords[4] = new Coord(coords[54].X, coords[54].Y);
                lipsCoords[5] = new Coord(coords[56].X, coords[56].Y);
                lipsCoords[6] = new Coord(coords[57].X, coords[57].Y);
                lipsCoords[7] = new Coord(coords[58].X, coords[58].Y);

                //For open mouth
                var innerMouthCoords = new Coord[8];
                innerMouthCoords[0] = new Coord(coords[48].X - 2, coords[48].Y);
                innerMouthCoords[1] = new Coord(coords[65].X, coords[65].Y);
                innerMouthCoords[2] = new Coord(coords[64].X, coords[64].Y);
                innerMouthCoords[3] = new Coord(coords[63].X, coords[63].Y);
                innerMouthCoords[4] = new Coord(coords[54].X - 2, coords[54].Y);
                innerMouthCoords[5] = new Coord(coords[62].X, coords[62].Y);
                innerMouthCoords[6] = new Coord(coords[61].X, coords[61].Y);
                innerMouthCoords[7] = new Coord(coords[60].X, coords[60].Y);

                //add all feature coords to result
                result.Add(SearchFeatureId.Face, new CvResult { Coords = faceCoords });
                result.Add(SearchFeatureId.LeftEye, new CvResult { Coords = leftEyeCoords });
                result.Add(SearchFeatureId.LeftEyeball, new CvEyeballResult
                    {
                        Radius = leftEyeballRadius,
                        PupilCoord = leftEyePupilCoord
                    });
                result.Add(SearchFeatureId.LeftEyeBrow, new CvResult { Coords = leftEyeBrowCoords });
                result.Add(SearchFeatureId.RightEye, new CvResult { Coords = rightEyeCoords});
                result.Add(SearchFeatureId.RightEyeball, new CvEyeballResult
                    {
                        Radius = rightEyeballRadius,
                        PupilCoord = rightEyePupilCoord
                    });
                result.Add(SearchFeatureId.RightEyeBrow, new CvResult { Coords = rightEyeBrowCoords });
                result.Add(SearchFeatureId.Nose, new CvResult { Coords = noseCoords });
                result.Add(SearchFeatureId.Lips, new CvResult { Coords = lipsCoords });
                result.Add(SearchFeatureId.OpenLips, new CvResult {Coords = innerMouthCoords});

                //    result = new List<CvSearchResult>
                //        {
                //            new CvSearchResult {Id = SearchFeatureId.Face, Coords = faceCoords},
                //            new CvSearchResult {Id = SearchFeatureId.LeftEye, Coords = leftEyeCoords},
                //            new CvEyeballResult
                //                {
                //                    Id = SearchFeatureId.LeftEyeball, Coords=new Coord[]{},
                //                    Radius = leftEyeballRadius,
                //                    PupilLocation = leftEyePupilCoord
                //                },
                //            new CvSearchResult {Id = SearchFeatureId.LefttEyeBrow, Coords = leftEyeBrowCoords},
                //            new CvSearchResult {Id = SearchFeatureId.RightEye, Coords = rightEyeBrowCoords},
                //            new CvEyeballResult
                //                {
                //                    Id = SearchFeatureId.RightEyeball, Coords=new Coord[]{},
                //                    Radius = rightEyeballRadius,
                //                    PupilLocation = rightEyePupilCoord
                //                },
                //            new CvSearchResult {Id = SearchFeatureId.RightEyeBrow, Coords = rightEyeBrowCoords},
                //            new CvSearchResult {Id = SearchFeatureId.Nose, Coords = noseCoords},
                //            new CvSearchResult {Id = SearchFeatureId.Lips, Coords = lipsCoords},
                //            new CvSearchResult {Id = SearchFeatureId.OpenMouth, Coords = innerMouthCoords},
                //        };
            }
            
            return result;
        }
    }
}