using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Options;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.RequestLocalization
{
    public class AbpRequestLocalizationOptionsManager : SkywalkerDynamicOptionsManager<RequestLocalizationOptions>
    {
        private RequestLocalizationOptions _options;

        private readonly IAbpRequestLocalizationOptionsProvider _abpRequestLocalizationOptionsProvider;

        public AbpRequestLocalizationOptionsManager(
            IOptionsFactory<RequestLocalizationOptions> factory,
            IAbpRequestLocalizationOptionsProvider abpRequestLocalizationOptionsProvider)
            : base(factory)
        {
            _abpRequestLocalizationOptionsProvider = abpRequestLocalizationOptionsProvider;
        }

        public override RequestLocalizationOptions Get(string name)
        {
            return _options ?? base.Get(name);
        }

        protected override async Task OverrideOptionsAsync(string name, RequestLocalizationOptions options)
        {
            _options = await _abpRequestLocalizationOptionsProvider.GetLocalizationOptionsAsync();
        }
    }
}
