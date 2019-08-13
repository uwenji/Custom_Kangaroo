using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CustomGoal
{
    public class CustomGoalInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CustomKangarooGoal";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Base Kangaroo Library";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("bd28e851-cfc6-4446-a32f-3b6bb749a58f");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "You-Wen Ji";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "ohisyouwen.ji@gmail.com";
            }
        }
    }
}
