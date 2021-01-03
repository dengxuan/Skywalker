﻿// Copyright 2004-2011 Hermit Project - http://www.Hermitproject.org/
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

namespace Skywalker.Aspects.DynamicProxy
{

    [Serializable]
    public class InvalidProxyConstructorArgumentsException : ArgumentException
    {
        public InvalidProxyConstructorArgumentsException(string message, Type proxyType, Type classToProxy) : base(message)
        {
            ProxyType = proxyType;
            ClassToProxy = classToProxy;
        }

        public Type ClassToProxy { get; private set; }

        public Type ProxyType { get; private set; }
    }
}