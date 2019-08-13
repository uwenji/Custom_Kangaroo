using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using KPlankton;
using KangarooSolver;
using KangarooSolver.Goals;


namespace CustomGoal
{
    public class LockHingeComponent : GH_Component
    {
        Mesh M;
        double Strength;
        Interval Range = Interval.Unset;

        GoalObject[] goals;

        public LockHingeComponent()
          : base("LockHinge", "LockHinge",
              "[Custom]:Lock hinge angle in given range",
              "Kangaroo2", "Goals-Mesh")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Angle", "A", "Angle Range (Radian)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Strength", GH_ParamAccess.item, 1);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Goal", "G", "Goal", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            M = new Mesh();

            DA.GetData<Mesh>(0, ref M);
            DA.GetData<Interval>(1, ref Range);
            DA.GetData<double>(2, ref Strength);
            List<List<Point3d>> PPS = CustomGoal.Goals.Util.HingeVertices(M);

            goals = new GoalObject[PPS.Count];

            for (int i = 0; i < goals.Length; i ++)
            {
                List<Point3d> P = PPS[i];
                goals[i] = new Goals.LockMeshAngle(P[0], P[1], P[2], P[3], Range, Strength);
            }

            
            DA.SetDataList(0, goals);
            
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            { 
                return Properties.Resources.LockHinge;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("6b7a445b-85a5-4eda-9868-bcbb91c5e189"); }
        }
    }
}
