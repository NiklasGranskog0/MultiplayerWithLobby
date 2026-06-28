using Project_Assets.Scripts.Enums;

namespace Project_Assets.Scripts.Interfaces
{
    public interface ISelectionObject
    {
        public ImageToLoad ImageToLoad { get; }
        public string Name { get; }
    }
}