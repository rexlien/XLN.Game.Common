using System;
using System.Threading.Tasks;
namespace XLN.Game.Common
{
    public interface IResource : ICacheable, IStreamable
    {
        bool Load(ResourcePath path);
        Task<bool> LoadAsync(ResourcePath path);


        Converter<R> GetConverter<R>();

    };
}
