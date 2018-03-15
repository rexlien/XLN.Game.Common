using System;
namespace XLN.Game.Common
{
    public interface IConverter
    {
        
    }

    public class Converter<T> : IConverter
    {

        public virtual T Convert()
        {
            return default(T);
        }

    }
}
