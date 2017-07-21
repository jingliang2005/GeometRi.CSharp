﻿using System;
using static System.Math;

namespace GeometRi
{
    public class Segment3d
    {

        private Point3d _p1;
        private Point3d _p2;

        public Segment3d(Point3d p1, Point3d p2)
        {
            _p1 = p1.Copy();
            _p2 = p2.ConvertTo(p1.Coord);
        }

        /// <summary>
        /// Creates copy of the object
        /// </summary>
        public Segment3d Copy()
        {
            return new Segment3d(_p1,_p2);
        }

        public Point3d P1
        {
            get { return _p1.Copy(); }
            set { _p1 = value.Copy(); }
        }

        public Point3d P2
        {
            get { return _p2.Copy(); }
            set { _p2 = value.Copy(); }
        }

        public double Length
        {
            get { return _p1.DistanceTo(_p2); }
        }

        public Vector3d ToVector
        {
            get { return new Vector3d(_p1, _p2); }
        }

        public Ray3d ToRay
        {
            get { return new Ray3d(_p1, new Vector3d(_p1, _p2)); }
        }

        public Line3d ToLine
        {
            get { return new Line3d(_p1, _p2); }
        }

        #region "DistanceTo"
        /// <summary>
        /// Returns shortest distance from segment to the point
        /// </summary>
        public double DistanceTo(Point3d p)
        {
            return p.DistanceTo(this);
        }

        /// <summary>
        /// Returns shortest distance from segment to the plane
        /// </summary>
        public double DistanceTo(Plane3d s)
        {

            object obj = this.IntersectionWith(s);

            if (obj == null)
            {
                return Min(this.P1.DistanceTo(s), this.P2.DistanceTo(s));
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns shortest distance from segment to the line
        /// </summary>
        public double DistanceTo(Line3d l)
        {
            if (l.PerpendicularTo(this.ToLine).BelongsTo(this))
            {
                return l.DistanceTo(this.ToLine);
            }
            else
            {
                return Min(this.P1.DistanceTo(l), this.P2.DistanceTo(l));
            }
        }

        /// <summary>
        /// Returns shortest distance between two segments
        /// </summary>
        public double DistanceTo(Segment3d s)
        {

            // Algorithm by Dan Sunday
            // http://geomalgorithms.com/a07-_distance.html

            double small = 1E-09;

            Vector3d u = this.ToVector;
            Vector3d v = s.ToVector;
            Vector3d w = new Vector3d(s.P1, this.P1);

            double a = u * u;
            double b = u * v;
            double c = v * v;
            double d = u * w;
            double e = v * w;

            double DD = a * c - b * b;
            double sc = 0;
            double sN = 0;
            double sD = 0;
            double tc = 0;
            double tN = 0;
            double tD = 0;
            sD = DD;
            tD = DD;

            if (DD < small)
            {
                // the lines are almost parallel, force using point Me.P1 to prevent possible division by 0.0 later
                sN = 0.0;
                sD = 1.0;
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if ((sN < 0.0))
                {
                    // sc < 0 => the s=0 edge Is visible
                    sN = 0.0;
                    tN = e;
                    tD = c;
                }
                else if ((sN > sD))
                {
                    // sc > 1  => the s=1 edge Is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if ((tN < 0.0))
            {
                // tc < 0 => the t=0 edge Is visible
                tN = 0.0;
                // recompute sc for this edge
                if ((-d < 0.0))
                {
                    sN = 0.0;
                }
                else if ((-d > a))
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if ((tN > tD))
            {
                // tc > 1  => the t=1 edge Is visible
                tN = tD;
                // recompute sc for this edge
                if (((-d + b) < 0.0))
                {
                    sN = 0;
                }
                else if (((-d + b) > a))
                {
                    sN = sD;
                }
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            // finally do the division to get sc And tc
            sc = Abs(sN) < small ? 0.0 : sN / sD;
            tc = Abs(tN) < small ? 0.0 : tN / tD;

            // get the difference of the two closest points
            Vector3d dP = w + (sc * u) - (tc * v);
            // =  S1(sc) - S2(tc)

            return dP.Norm;

        }

        /// <summary>
        /// Returns shortest distance from segment to ray
        /// </summary>
        public double DistanceTo(Ray3d r)
        {

            if (this.ToVector.IsParallelTo(r.Direction))
                return this.ToLine.DistanceTo(r.ToLine);

            if (this.ToLine.PerpendicularTo(r.ToLine).BelongsTo(r) && r.ToLine.PerpendicularTo(this.ToLine).BelongsTo(this))
            {
                return this.ToLine.DistanceTo(r.ToLine);
            }

            double d1 = double.PositiveInfinity;
            double d2 = double.PositiveInfinity;
            double d3 = double.PositiveInfinity;
            bool flag = false;

            if (r.Point.ProjectionTo(this.ToLine).BelongsTo(this))
            {
                d1 = r.Point.DistanceTo(this.ToLine);
                flag = true;
            }
            if (this.P1.ProjectionTo(r.ToLine).BelongsTo(r))
            {
                d2 = this.P1.DistanceTo(r.ToLine);
                flag = true;
            }
            if (this.P2.ProjectionTo(r.ToLine).BelongsTo(r))
            {
                d3 = this.P2.DistanceTo(r.ToLine);
                flag = true;
            }

            if (flag)
                return Min(d1, Min(d2, d3));

            return Min(this.P1.DistanceTo(r.Point), this.P2.DistanceTo(r.Point));

        }
        #endregion

        /// <summary>
        /// Get intersection of segment with plane.
        /// Returns object of type 'Nothing', 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Plane3d s)
        {

            object obj = this.ToRay.IntersectionWith(s);

            if (obj == null)
            {
                return null;
            }
            else
            {
                if (object.ReferenceEquals(obj.GetType(), typeof(Ray3d)))
                {
                    return this;
                }
                else
                {
                    Ray3d r = new Ray3d(this.P2, new Vector3d(this.P2, this.P1));
                    object obj2 = r.IntersectionWith(s);
                    if (obj2 == null)
                    {
                        return null;
                    }
                    else
                    {
                        return (Point3d)obj2;
                    }
                }
            }
        }

        /// <summary>
        /// Get the orthogonal projection of a segment to the line.
        /// Return object of type 'Segment3d' or 'Point3d'
        /// </summary>
        public object ProjectionTo(Line3d l)
        {
            if (this.ToVector.IsOrthogonalTo(l.Direction))
            {
                // Segment is perpendicular to the line
                return this.P1.ProjectionTo(l);
            }
            else
            {
                return new Segment3d(this.P1.ProjectionTo(l), this.P2.ProjectionTo(l));
            }
        }

        /// <summary>
        /// Get the orthogonal projection of a segment to the plane.
        /// Return object of type 'Segment3d' or 'Point3d'
        /// </summary>
        public object ProjectionTo(Plane3d s)
        {
            if (this.ToVector.IsParallelTo(s.Normal))
            {
                // Segment is perpendicular to the plane
                return this.P1.ProjectionTo(s);
            }
            else
            {
                return new Segment3d(this.P1.ProjectionTo(s), this.P2.ProjectionTo(s));
            }
        }

        #region "AngleTo"
        /// <summary>
        /// Angle between segment and plane in radians (0 &lt; angle &lt; Pi/2)
        /// </summary>
        public double AngleTo(Plane3d s)
        {
            double ang = Asin(this.ToVector.Dot(s.Normal) / this.ToVector.Norm / s.Normal.Norm);
            return Abs(ang);
        }
        /// <summary>
        /// Angle between segment and plane in degrees (0 &lt; angle &lt; 90)
        /// </summary>
        public double AngleToDeg(Plane3d s)
        {
            return AngleTo(s) * 180 / PI;
        }
        #endregion


        #region "TranslateRotateReflect"
        /// <summary>
        /// Translate segment by a vector
        /// </summary>
        public Segment3d Translate(Vector3d v)
        {
            return new Segment3d(P1.Translate(v), P2.Translate(v));
        }

        /// <summary>
        /// Rotate segment by a given rotation matrix
        /// </summary>
        public virtual Segment3d Rotate(Matrix3d m)
        {
            return new Segment3d(P1.Rotate(m), P2.Rotate(m));
        }

        /// <summary>
        /// Rotate segment by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        public virtual Segment3d Rotate(Matrix3d m, Point3d p)
        {
            return new Segment3d(P1.Rotate(m, p), P2.Rotate(m, p));
        }

        /// <summary>
        /// Reflect segment in given point
        /// </summary>
        public virtual Segment3d ReflectIn(Point3d p)
        {
            return new Segment3d(P1.ReflectIn(p), P2.ReflectIn(p));
        }

        /// <summary>
        /// Reflect segment in given line
        /// </summary>
        public virtual Segment3d ReflectIn(Line3d l)
        {
            return new Segment3d(P1.ReflectIn(l), P2.ReflectIn(l));
        }

        /// <summary>
        /// Reflect segment in given plane
        /// </summary>
        public virtual Segment3d ReflectIn(Plane3d s)
        {
            return new Segment3d(P1.ReflectIn(s), P2.ReflectIn(s));
        }
        #endregion

        /// <summary>
        /// Determines whether two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }
            Segment3d s = (Segment3d)obj;
            return (this.P1 == s.P1 && this.P2 == s.P2) | (this.P1 == s.P2 && this.P2 == s.P1);
        }

        /// <summary>
        /// Returns the hascode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_p1.GetHashCode(), _p2.GetHashCode());
        }

        /// <summary>
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override String ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }

        /// <summary>
        /// String representation of an object in reference coordinate system.
        /// </summary>
        public String ToString(Coord3d coord)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d p1 = _p1.ConvertTo(coord);
            Point3d p2 = _p2.ConvertTo(coord);

            str.Append("Segment:" + nl);
            str.Append(string.Format("Point 1  -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p1.X, p1.Y, p1.Z) + nl);
            str.Append(string.Format("Point 2 -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", p2.X, p2.Y, p2.Z));
            return str.ToString();
        }

        // Operators overloads
        //-----------------------------------------------------------------
        public static bool operator ==(Segment3d l1, Segment3d l2)
        {
            return l1.Equals(l2);
        }
        public static bool operator !=(Segment3d l1, Segment3d l2)
        {
            return !l1.Equals(l2);
        }

    }
}

