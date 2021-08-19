using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer
{
    public class TransferOptions
    {
        public IServiceCollection Services { get; set; }
    }
}
