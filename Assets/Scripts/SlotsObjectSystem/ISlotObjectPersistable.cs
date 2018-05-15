using System.Runtime.Serialization;

namespace Assets.Scripts.SlotsObjectSystem
{
    public interface ISlotObjectPersistable: ISerializable
    {
        void SaveChanges();
        ISlotObjectPersistable Load();
    }
}
