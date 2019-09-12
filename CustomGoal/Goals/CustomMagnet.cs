public class MagnetGoal : GoalObject
  {
        /*TODO:
         * [0]:find closet points
         * [1]:apply force to point
         */
    public double Strength;
    public double Radius;
    public RTree rTree;
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
      rTree = new RTree();

      int L = PIndex.Length;

      for (int i = 0; i < L; i++)
      {
        rTree.Insert(p[PIndex[i]].Position, i);
        Move[i] = Vector3d.Zero;
        Weighting[i] = 0;
      }

      for (int i = 0; i < L; i++)
      {
        KangarooSolver.Particle pos = p[i];
        List<Point3d> Neighbours = new List<Point3d>();

        EventHandler<RTreeEventArgs> rTreeCallback =
          (object sender, RTreeEventArgs args) =>
          {
          if (p[args.Id] != pos)
          {
            Neighbours.Add(p[args.Id].Position);
          }

          };

        rTree.Search(new Sphere(pos.Position, Radius), rTreeCallback);
        Weighting[i] = Strength;
        if(Neighbours.Count > 0)
          Move[i] = computeMagnet(pos.Position, Neighbours);
      }
    }

    static public Vector3d computeMagnet(Point3d self, List<Point3d> points)
    {
      Point3d mass = Point3d.Origin;
      foreach(Point3d p in points){
        mass += p / (points.Count);
      }

      return (mass - self)/points.Count;
    }
  }
