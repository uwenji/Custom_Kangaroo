using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using KPlankton;
using KangarooSolver;
using KangarooSolver.Goals;

namespace CustomGoal.Components
{
    public class DynamicBorder : GH_Component
    {
        List<Point3d> points = new List<Point3d>();
        Polyline border;
        Curve conver_C;
        double strength;

        public DynamicBorder()
          : base("DynamicBorder", "DynamicBorder",
              "[Custom] Keep points in border mesh, border allow be move in solver.\n\rby You-Wen Ji,ohisyouwen.ji@gmail.com ",
              "Kangaroo2", "Goals-Col")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("BouncyPoint", "Points", "bouncy points in border", GH_ParamAccess.list);
            pManager.AddCurveParameter("Border", "Border", "Polyline border", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "Strength", "Strength", GH_ParamAccess.item, 1);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Goal", "G", "BorderGoal", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            points.Clear();
            
            DA.GetDataList<Point3d>(0, points);
            DA.GetData<Curve>(1, ref conver_C);
            DA.GetData<double>(2, ref strength);

            Mesh pmc = Goals.Util.pointsToDeluanayMesh(points);
            conver_C.TryGetPolyline(out border);
            GoalObject goal = new Goals.DynamicBorder(pmc, border, strength);

            DA.SetData(0, goal);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.borderGoal;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1f3866ac-564b-455d-a0c3-2cb564be4f4b"); }
        }
    }
}