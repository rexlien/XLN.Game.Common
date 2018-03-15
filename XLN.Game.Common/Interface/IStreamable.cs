using System.IO;

namespace XLN.Game.Common
{
    
    public interface IStreamable
    {
        
        R Deserialize<R>(string key = null);

        //R Deserialize<R>(Stream stream);
    }

}