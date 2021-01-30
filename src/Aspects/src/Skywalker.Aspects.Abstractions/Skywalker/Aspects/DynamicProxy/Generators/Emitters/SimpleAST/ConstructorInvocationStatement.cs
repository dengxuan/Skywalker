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

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Skywalker.Aspects.DynamicProxy.Generators.Emitters.SimpleAST
{
    public class ConstructorInvocationStatement : Statement
    {
        private readonly Expression[] args;
        private readonly ConstructorInfo cmethod;

        public ConstructorInvocationStatement(ConstructorInfo method, params Expression[] args)
        {
            cmethod = method ?? throw new ArgumentNullException("method");
            this.args = args ?? throw new ArgumentNullException("args");
        }

        public override void Emit(IMemberEmitter member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);

            foreach (var exp in args)
            {
                exp.Emit(member, gen);
            }

            gen.Emit(OpCodes.Call, cmethod);
        }
    }
}