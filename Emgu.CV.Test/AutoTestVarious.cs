using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Emgu.CV;
using Emgu.UI;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

namespace Emgu.CV.Test
{
    [TestFixture]
    public class AutoTestVarious
    {
        [Test]
        public void TestColorEqual()
        {
            Bgr c1 = new Bgr(0.0, 0.0, 0.0);
            Bgr c2 = new Bgr(0.0, 0.0, 0.0);
            Assert.IsTrue(c1.Equals(c2));
        }

        [Test]
        public void TestCvClipLine()
        {
            MCvPoint m1 = new MCvPoint(-1, 10);
            MCvPoint m2 = new MCvPoint(100, 10);
            int inside = CvInvoke.cvClipLine(new MCvSize(20, 20), ref m1, ref m2);
            Assert.AreEqual(0, m1.x);
            Assert.AreEqual(19, m2.x);
        }

        [Test]
        public void TestLookup()
        {
            double[] b = new double[4] { 0.0, 1.0, 2.0, 3.0 };
            double[] a = new double[4] { 1.0, 3.0, 2.0, 0.0 };
            Point2D<double>[] pts = new Point2D<double>[b.Length];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = new Point2D<double>(b[i], a[i]);

            Assert.AreEqual(2.5, PointCollection<double>.FirstDegreeInterpolate(pts, 1.5));
            Assert.AreEqual(-1, PointCollection<double>.FirstDegreeInterpolate(pts, 3.5));
        }

        [Test]
        public void TestLineFitting()
        {
            List<Point2D<float>> pts = new List<Point2D<float>>();

            pts.Add(new Point2D<float>(1.0f, 1.0f));
            pts.Add(new Point2D<float>(2.0f, 2.0f));
            pts.Add(new Point2D<float>(3.0f, 3.0f));
            pts.Add(new Point2D<float>(4.0f, 4.0f));

            Line2D<float> res = PointCollection<float>.Line2DFitting((IEnumerable<Point<float>>)pts.ToArray(), Emgu.CV.CvEnum.DIST_TYPE.CV_DIST_L2);

            //check if the line is 45 degree from +x axis
            Assert.AreEqual(45.0, res.Direction.PointDegreeAngle);
        }

        [Test]
        public void TestSerialization()
        {
            Rectangle<int> rec = new Rectangle<int>(-10, 10, 10, -2);
            XmlDocument xdoc = Emgu.Utils.XmlSerialize<Rectangle<int>>(rec);
            //Trace.WriteLine(xdoc.OuterXml);
            rec = Emgu.Utils.XmlDeserialize<Rectangle<int>>(xdoc);

            Point2D<double> pt2d = new Point2D<double>(12.0, 5.5);
            xdoc = Emgu.Utils.XmlSerialize<Point2D<double>>(pt2d);
            //Trace.WriteLine(xdoc.OuterXml);
            pt2d = Emgu.Utils.XmlDeserialize<Point2D<double>>(xdoc);

            Circle<float> cir = new Circle<float>(new Point2D<float>(0.0f, 1.0f), 2.8f);
            xdoc = Emgu.Utils.XmlSerialize<Circle<float>>(cir);
            //Trace.WriteLine(xdoc.OuterXml);
            cir = Emgu.Utils.XmlDeserialize<Circle<float>>(xdoc);

            Image<Bgr, Byte> img1 = new Image<Bgr, byte>("stuff.jpg");
            xdoc = Emgu.Utils.XmlSerialize(img1);
            //Trace.WriteLine(xdoc.OuterXml);
            Image<Bgr, Byte> img2 = Emgu.Utils.XmlDeserialize<Image<Bgr, Byte>>(xdoc);

            Byte[] a1 = img1.Bytes;
            Byte[] a2 = img2.Bytes;
            Assert.AreEqual(a1.Length, a2.Length);
            for (int i = 0; i < a1.Length; i++)
            {
                Assert.AreEqual(a1[i], a2[i]);
            }

            img1.Dispose();
            img2.Dispose();
        }

        [Test]
        public void TestRotationMatrix3D()
        {
            float[] rod = new float[] { 0.2f, 0.5f, 0.3f };
            RotationVector rodVec = new RotationVector(rod);

            RotationVector rodVec2 = new RotationVector();
            rodVec2.RotationMatrix = rodVec.RotationMatrix;
            Assert.IsTrue(rodVec.Equals(rodVec2));
        }

        [Test]
        public void TestContour()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Gray, Byte> img = new Image<Gray, Byte>(100, 100, new Gray()))
            {
                Rectangle<double> rect = new Rectangle<double>(10, 80, 50, 10);
                img.Draw(rect, new Gray(255.0), -1);

                using (MemStorage stor = new MemStorage())
                {
                    Contour cs = img.FindContours(CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, CvEnum.RETR_TYPE.CV_RETR_LIST, stor);
                    Assert.AreEqual(rect.Area, cs.Area);
                    Assert.IsTrue(cs.Convex);
                    Assert.AreEqual(rect.Width * 2 + rect.Height * 2, cs.Perimeter);
                    Rectangle<double> rect2 = cs.BoundingRectangle;
                    rect2.Width -= 1;
                    rect2.Height -= 1;
                    rect2.Center.X -= 0.5;
                    rect2.Center.Y -= 0.5;
                    Assert.IsTrue(rect2.Equals(rect));
                }
            }
        }

        [Test]
        public void TestException()
        {
            for (int i = 0; i < 10; i++)
            {
                bool exceptionCaught = false;
                Matrix<Byte> mat = new Matrix<byte>(20, 30);
                try
                {
                    double det = mat.Det;
                }
                catch (CvException excpt)
                {
                    Assert.AreEqual(-201, excpt.Status);
                    exceptionCaught = true;
                }
                Assert.IsTrue(exceptionCaught);
            }
        }

        [Test]
        public void TestRectangle()
        {
            Matrix<Byte> mat = new Matrix<Byte>(1, 4);
            mat._RandUniform(new MCvScalar(), new MCvScalar(255.0));

            MCvRect rect1 = new MCvRect((int)mat[0, 0], (int)mat[0, 1], (int)mat[0, 2], (int)mat[0, 3]);
            Rectangle<double> rectangle = new Rectangle<double>(rect1);
            MCvRect rect2 = rectangle.MCvRect;

            Assert.AreEqual(rect1.x, rect2.x);
            Assert.AreEqual(rect1.y, rect2.y);

        }
    }
}
