using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.Scripts.Contracts
{
    public interface ISlotObjectPersistable: ISerializable
    {
        void SaveChanges();
        ISlotObjectPersistable Load();
    }
}
