﻿using System;
using static System.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeometRi;

namespace GeometRi_Tests
{
    [TestClass]
    public class Ray3dTest
    {
        //===============================================================
        // Ray3d tests
        //===============================================================

        [TestMethod()]
        public void RayDistanceToCrossingLineTest()
        {
            Line3d l = new Line3d(new Point3d(0, 0, 0), new Vector3d(1, 0, 0));
            Ray3d r = new Ray3d(new Point3d(2, -3, 3), new Vector3d(0, 1, 0));
            Assert.IsTrue(Abs(r.DistanceTo(l) - 3) < GeometRi3D.Tolerance);
        }

        [TestMethod()]
        public void RayDistanceToCrossingLineTest2()
        {
            Line3d l = new Line3d(new Point3d(0, 0, 0), new Vector3d(1, 0, 0));
            Ray3d r = new Ray3d(new Point3d(2, 4, 3), new Vector3d(0, 1, 0));
            Assert.IsTrue(Abs(r.DistanceTo(l) - 5) < GeometRi3D.Tolerance);
        }

        [TestMethod()]
        public void RayIntersectionWithPlaneTest()
        {
            Plane3d s = new Plane3d(0, 0, 1, 0);
            Ray3d r = new Ray3d(new Point3d(-1, -1, -1), new Vector3d(1, 1, 1));
            Assert.IsTrue((Point3d)r.IntersectionWith(s) == new Point3d(0, 0, 0));
        }

        [TestMethod()]
        public void RayIntersectionWithPlaneTest2()
        {
            Plane3d s = new Plane3d(0, 0, 1, 0);
            Ray3d r = new Ray3d(new Point3d(1, 1, 1), new Vector3d(1, 1, 1));
            Assert.IsTrue(r.IntersectionWith(s) == null);
        }

        [TestMethod()]
        public void RayProjectionToPlaneTest()
        {
            Plane3d s = new Plane3d(0, 0, 1, 0);
            Ray3d r = new Ray3d(new Point3d(1, 1, 1), new Vector3d(1, 1, 1));
            Assert.IsTrue((Ray3d)r.ProjectionTo(s) == new Ray3d(new Point3d(1, 1, 0), new Vector3d(1, 1, 0)));
        }

        [TestMethod()]
        public void RayDistanceToRayTest()
        {
            Ray3d r1 = new Ray3d(new Point3d(0, 0, 0), new Vector3d(1, 0, 0));
            Ray3d r2 = new Ray3d(new Point3d(5, 0, 5), new Vector3d(-1, 0, 0));
            Assert.IsTrue(Abs(r1.DistanceTo(r2) - 5) < GeometRi3D.Tolerance);

            r2 = new Ray3d(new Point3d(5, -5, 5), new Vector3d(0, 1, 0));
            Assert.IsTrue(Abs(r1.DistanceTo(r2) - 5) < GeometRi3D.Tolerance);

            r2 = new Ray3d(new Point3d(-5, 0, 5), new Vector3d(0, 1, 1));
            Assert.IsTrue(Abs(r1.DistanceTo(r2) - Sqrt(50)) < GeometRi3D.Tolerance);
        }

    }
}
