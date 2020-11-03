using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Localization
{
    public interface ILanguageProvider
    {
        Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync();
    }
}
