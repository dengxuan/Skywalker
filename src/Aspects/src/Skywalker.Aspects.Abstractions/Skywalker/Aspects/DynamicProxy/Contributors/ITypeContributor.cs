// Copyright 2004-2011 Skywalker Project
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Skywalker.Aspects.DynamicProxy.Generators;
using Skywalker.Aspects.DynamicProxy.Generators.Emitters;
using Skywalker.Aspects.DynamicProxy;

namespace Skywalker.Aspects.DynamicProxy.Contributors
{
    /// <summary>
    ///   Interface describing elements composing generated type
    /// </summary>
    public interface ITypeContributor
	{
		void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model);

		void Generate(ClassEmitter @class, ProxyGenerationOptions options);
	}
}