using System.IO;

namespace XLN.Game.Common
{
    
    public interface IStreamable<T>
    {
        
        T Deserialize(string key = null);
        //T Deserialize();
        T Deserialize(Stream stream);
    }

}