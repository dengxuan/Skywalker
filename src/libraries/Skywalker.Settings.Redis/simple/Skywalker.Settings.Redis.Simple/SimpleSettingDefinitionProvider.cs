// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Skywalker.Settings.Abstractions;
using Volo.Abp.Settings;

namespace Skywalker.Settings.Redis.Simple;
internal class SimpleSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(new SettingDefinition("UserName","张婉"));
    }
}
