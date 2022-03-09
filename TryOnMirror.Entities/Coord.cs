namespace SymaCord.TryOnMirror.Entities
{
   public class Coord
    {
       public Coord(){}

       public float X { get; set; }

       public float Y { get; set; }

       public Coord(float x, float y)
       {
           this.X = x;
           this.Y = y;
       }
    }
}
