namespace zbrozonoid.Views
{
    public interface IGamePlayfieldView : IView
    {
        void PrepareBackground(string backgroundName);
        void PrepareBricksToDraw();
        void BrickHit(int number);
    }
}
