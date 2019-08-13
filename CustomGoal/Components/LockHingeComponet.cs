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

        public LockHingeComponent()
          : base("LockHinge", "LockHinge",
              "Lock hinge angle in given range",
              "Kangaroo", "Goals-Mesh")
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
