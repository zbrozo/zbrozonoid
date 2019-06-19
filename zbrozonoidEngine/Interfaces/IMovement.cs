namespace zbrozonoidEngine.Interfaces
{
    public interface IMovement
    {
        int Iteration { get; set; }
        int Degree { get; set; }
        Vector2 Offset { get; set; }
        Vector2 Direction { get; set; }

        bool Move(out Vector2 position);
    }
}
