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
using System.Reflection.Emit;

namespace Skywalker.Aspects.DynamicProxy.Generators.Emitters.SimpleAST
{
    public class NullCoalescingOperatorExpression : Expression
    {
        private readonly Expression @default;
        private readonly Expression expression;

        public NullCoalescingOperatorExpression(Expression expression, Expression @default)
        {
            this.expression = expression ?? throw new ArgumentNullException("expression");
            this.@default = @default ?? throw new ArgumentNullException("default");
        }

        public override void Emit(IMemberEmitter member, ILGenerator gen)
        {
            expression.Emit(member, gen);
            gen.Emit(OpCodes.Dup);
            var label = gen.DefineLabel();
            gen.Emit(OpCodes.Brtrue_S, label);
            gen.Emit(OpCodes.Pop);
            @default.Emit(member, gen);
            gen.MarkLabel(label);
        }
    }
}