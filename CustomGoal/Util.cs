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
    public class Util
    {

        //get hinge 4 points from mesh
        public static List<List<Point3d>> HingeVertices(Mesh M)
        {
            List<List<Point3d>> points = new List<List<Point3d>>();
            KPlanktonMesh km = M.ToKPlanktonMesh();

            for (int i = 0; i < km.Halfedges.Count; i += 2)
            {
                if (km.Halfedges.IsBoundary(i))
                    continue;

                List<Point3d> thisPs = new List<Point3d>();
                int nEi = km.Halfedges[i + 1].NextHalfedge; // next edge id
                int nPi = km.Halfedges[i].PrevHalfedge; // next edge id
                                                        // foldStart
                thisPs.Add(km.Vertices[km.Halfedges[i].StartVertex].ToPoint3d());
                // foldEnd
                thisPs.Add(km.Vertices[km.Halfedges[i + 1].StartVertex].ToPoint3d());
                // tipA
                thisPs.Add(km.Vertices[km.Halfedges[nEi + 1].StartVertex].ToPoint3d());
                // tipB
                thisPs.Add(km.Vertices[km.Halfedges[nPi].StartVertex].ToPoint3d());
                points.Add(thisPs);
            }

            return points;
        }

        //create a mesh from polyline by kangaroo plankton mesh
        public static Mesh polylineToMesh(Polyline C)
        {
            KPlanktonMesh KM = new KPlanktonMesh();
            var points = new Rhino.Collections.Point3dList();

            var faceIndices = new List<int>();
            for (int i = 0; i < C.SegmentCount; i++)
            {
                int nearest = points.ClosestIndex(C.PointAt(i));
                if (nearest == -1)
                {
                    faceIndices.Add(KM.Vertices.Count);
                    KM.Vertices.Add(C.PointAt(i));
                    points.Add(C.PointAt(i));
                }
                else
                {
                    if (points[nearest].DistanceTo(C.PointAt(i)) < 1e-4)
                    {
                        faceIndices.Add(nearest);
                    }
                    else
                    {
                        faceIndices.Add(KM.Vertices.Count);
                        KM.Vertices.Add(C.PointAt(i));
                        points.Add(C.PointAt(i));
                    }
                }
            }
            KM.Faces.AddFace(faceIndices);

            return KM.ToRhinoMesh();
        }

        //get plane and bounding box from points
        public static Plane polylineBbox(List<Point3d> Points, out Box Bbox)
        {
            Polyline boundingCrv = new Polyline(Points);
            boundingCrv.Add(Points[0]);
            Line longest = boundingCrv.SegmentAt(0);
            Plane bPlane = new Plane();
            for (int i = 0; i < boundingCrv.SegmentCount; i++)
            {
                if (longest.Length < boundingCrv.SegmentAt(i).Length)
                {
                    longest = boundingCrv.SegmentAt(i);
                    if (i == 0)
                        bPlane = new Plane(longest.From, longest.Direction, -boundingCrv.SegmentAt(boundingCrv.SegmentCount - 1).Direction);
                    else
                        bPlane = new Plane(longest.From, longest.Direction, -boundingCrv.SegmentAt(i - 1).Direction);
                }
            }
            PolylineCurve boundingPolyCrv = boundingCrv.ToPolylineCurve();
            boundingPolyCrv.GetBoundingBox(bPlane, out Bbox);
            bPlane.Origin = (longest.To - longest.From) / 2 + longest.From;
            return bPlane;
        }

        //get plane and bounding box from polyline
        public static Plane polylineBbox(Polyline BoundingCrv, out Box Bbox)
        {
            Line longest = BoundingCrv.SegmentAt(0);
            Plane bPlane = new Plane();
            for (int i = 0; i < BoundingCrv.SegmentCount; i++)
            {
                if (longest.Length <= BoundingCrv.SegmentAt(i).Length)
                {
                    longest = BoundingCrv.SegmentAt(i);
                    if (i == 0)
                        bPlane = new Plane(longest.From, longest.Direction, -BoundingCrv.SegmentAt(BoundingCrv.SegmentCount - 1).Direction);
                    else
                        bPlane = new Plane(longest.From, longest.Direction, -BoundingCrv.SegmentAt(i - 1).Direction);
                }
            }
            PolylineCurve boundingPolyCrv = BoundingCrv.ToPolylineCurve();
            boundingPolyCrv.GetBoundingBox(bPlane, out Bbox);
            bPlane.Origin = (longest.To - longest.From) / 2 + longest.From;
            return bPlane;
        }
    }
}
