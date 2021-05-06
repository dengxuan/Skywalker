using Microsoft.AspNetCore.Mvc;
using Skywalker.AspNetCore.Mvc;

namespace Skywalker.Transfer.WebApi
{
    [Area("transfer")]
    [Route("api/transfer/[controller]")]
    public abstract class TransferController : SkywalkerController
    {
    }
}
