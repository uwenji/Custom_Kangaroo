using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using KPlankton;
using KangarooSolver;
using KangarooSolver.Goals;

namespace CustomGoal.Goals
{
    public class DynamicBorder : GoalObject
    {
        /*TODO:
         * [0]:get planes of border and bouncy points bounding box and align longest edge
         * [1]:orient border and put border and bouncy points into kangaroo particle array
         * [2]:get the border from kangaroo particle array, [B o r d e r + B o u n c y]
         * [3]:compute the move vector for border (following) and bouncy (pull)
         */
        public double Strength;
        public Mesh M;
        public Plane BPln;
        //TODO[1]:
        //points order is following naked edge
        public DynamicBorder() { }
        public DynamicBorder(Point3d[] P, Polyline C, double K)
        {
            Strength = K; 
            Mesh m = Util.polylineToMesh(C);
            Box borderBox;
            BPln = Util.polylineBbox(C, out borderBox);

            List<Point3d> pts = new List<Point3d>();
            foreach (Point3d pt in P)
            {
                pts.Add(pt);
            }
            Box collideBox;
            Plane collidePln = Util.polylineBbox(pts, out collideBox);
            Transform xform = Transform.PlaneToPlane(BPln, collidePln);
            m.Transform(xform);
            var mPoints = m.Vertices.ToPoint3dArray();
            //creat a array size mesh verices + points length
            var size = P.Length + mPoints.Length;
            PPos = new Point3d[size];

            Move = new Vector3d[size];
            Weighting = new double[size];

            //then using this structure recreate mesh in the kangaroo particles 
            for (int i = 0; i < mPoints.Length; i++)
            {
                PPos[i] = mPoints[i];
                Weighting[i] = K;
            }

            for (int i = mPoints.Length; i < PPos.Length; i++)
            {
                PPos[i] = P[i - mPoints.Length];
                Weighting[i] = K;
            }

            M = m;
        }
        //points order is z-like shape then using mesh as input
        public DynamicBorder(Mesh collideM, Polyline C, double K)
        {
            Strength = K;
            Mesh m = Util.polylineToMesh(C);
            Box borderBox;
            BPln = Util.polylineBbox(C, out borderBox);
            Point3d[] P = collideM.GetNakedEdges()[0].ToArray();

            List<Point3d> pts = new List<Point3d>();
            foreach (Point3d pt in P)
            {
                pts.Add(pt);
            }
            Box collideBox;

            Plane collidePln = Util.polylineBbox(pts, out collideBox);
            Transform xform = Transform.PlaneToPlane(BPln, collidePln);
            m.Transform(xform);
            var mPoints = m.Vertices.ToPoint3dArray();

            //creat a array size mesh verices + points length
            var size = P.Length + mPoints.Length;
            PPos = new Point3d[size];

            Move = new Vector3d[size];
            Weighting = new double[size];
            //Add mesh vertices to array[i][0.....]
            //then using this structure recreate mesh in the kangaroo particles 
            for (int i = 0; i < mPoints.Length; i++)
            {
                PPos[i] = mPoints[i];
                Weighting[i] = K;
            }

            for (int i = mPoints.Length; i < PPos.Length; i++)
            {
                PPos[i] = P[i - mPoints.Length];
                Weighting[i] = K;
            }

            M = m;
        }

        //TODO[2][3]
        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            //get plane of border mesh
            Polyline pCrv = new Polyline();
            Box borderBox;
            for (int i = 0; i < M.Vertices.Count; i++)
            {
                pCrv.Add(p[PIndex[i]].Position);
                Move[i] = Vector3d.Zero;
            }
            M = Util.polylineToMesh(pCrv);
            BPln = Util.polylineBbox(pCrv, out borderBox);

            //get plane of bouncy points inside/on border 
            List<Point3d> collidePoints = new List<Point3d>();
            Box collideBox;
            for (int i = M.Vertices.Count; i < PIndex.Length; i++)
            {
                collidePoints.Add(p[PIndex[i]].Position);
            }
            Plane collidePln = Util.polylineBbox(collidePoints, out collideBox);
            Transform xform = Transform.PlaneToPlane(BPln, collidePln);
            M.Transform(xform);

            //point[border vertices......, bouncy verticles.....]
            //assign move to border
            for (int i = 0; i < M.Vertices.Count; i++)
            {
                //Point3d bPt = p[PIndex[i]].Position;
                //bPt.Transform(xform);
                //Move[i] = 0.5 * (bPt - p[PIndex[i]].Position);
                //Move[i] = (bPt - p[PIndex[i]].Position);

                p[PIndex[i]].Position.X = M.Vertices[i].X;
                p[PIndex[i]].Position.Y = M.Vertices[i].Y;
                p[PIndex[i]].Position.Z = M.Vertices[i].Z;
                Weighting[i] = Strength;
            }

            //assign move to bouncy
            for (int i = M.Vertices.Count; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                if (ThisPt.DistanceTo(M.ClosestPoint(ThisPt)) >= 1e-5)
                {
                    var Push = 0.5 * (M.ClosestPoint(ThisPt) - ThisPt);
                    Move[i] = Push;
                    Weighting[i] = Strength;
                }
                else { Move[i] = Vector3d.Zero; Weighting[i] = Strength; }
            }
        }

        //show the border in kangaroo solver
        public override object Output(List<KangarooSolver.Particle> p)
        {
            return M.GetNakedEdges();
        }



        
    }



}
