public class MagnetGoal : GoalObject
  {
        /*TODO:
         * [0]:find closet points
         * [1]:apply force to point
         */
    public double Strength;
    public double Radius;

    //points order is z-like shape then using mesh as input
    public MagnetGoal(List<Point3d> P, double R, double K)
    {
      int L = P.Count;
      PPos = P.ToArray();
      Move = new Vector3d[L];
      Weighting = new double[L];
      Strength = K;
      for (int i = 0; i < L; i++)
      {
        Weighting[i] = K;
      }
      Radius = R;
    }

    // Calculate
    public override void Calculate(List<KangarooSolver.Particle> p)
    {
      int L = PIndex.Length;

      for (int i = 0; i < L; i++)
      {
        Move[i] = Vector3d.Zero;
        Weighting[i] = 0;
      }

      for (int i = 0; i < (PIndex.Length - 1); i++)
      {
        Point3d mass = new Point3d(0, 0, 0);
        List<int> id = new List<int>();
        int g = 0; //group count
        for (int j = i; j < PIndex.Length; j++)
        {
          double distance = p[PIndex[i]].Position.DistanceTo(p[PIndex[j]].Position);
          if (distance < Radius)
          {
            mass += p[PIndex[j]].Position;
            g += 1;
            id.Add(j);
          }
        }


        if (g > 0)
        {
          for (int j = 0; j < id.Count; j++)
          {
            Move[i] += (Move[id[j]] / g) * 0.1;
          }
          mass = new Point3d(mass.X / g, mass.Y / g, mass.Z / g);
          Move[i] += (mass - p[PIndex[i]].Position) / g;
          Weighting[i] = Strength;
        }
      }
    }

  }
