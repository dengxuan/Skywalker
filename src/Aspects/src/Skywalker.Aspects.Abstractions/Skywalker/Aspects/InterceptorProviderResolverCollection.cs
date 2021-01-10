﻿using Skywalker.Aspects.Abstractinons;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Skywalker.Aspects
{

    /// <summary>
    /// 
    /// </summary>                                                                                                  
    public class InterceptorProviderResolverCollection : IEnumerable<IInterceptorProviderResolver>
    {
        #region Fields
        private readonly List<IInterceptorProviderResolver> _resolverList;
        private readonly Dictionary<string, IInterceptorProviderResolver> _resolverDictionary;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorProviderResolverCollection"/> class.
        /// </summary>
        public InterceptorProviderResolverCollection()
        {
            _resolverList = new List<IInterceptorProviderResolver>();       
            _resolverDictionary = new Dictionary<string, IInterceptorProviderResolver>();
            //Add(nameof(AttributeInterceptorProviderResolver), new AttributeInterceptorProviderResolver());   
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified interceptor provider resolver.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="interceptorProviderResolver">The interceptor provider resolver.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">name</exception>
        public InterceptorProviderResolverCollection Add(string name, IInterceptorProviderResolver interceptorProviderResolver)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(interceptorProviderResolver, nameof(interceptorProviderResolver));
            if (_resolverDictionary.ContainsKey(name))
            {
                throw new ArgumentException("Duplicate InterceptorProviderResolver with the name {0} is registered.", nameof(name));
            }

            _resolverList.Add(interceptorProviderResolver);
            _resolverDictionary.Add(name, interceptorProviderResolver);
            return this;
        }

        /// <summary>
        /// Inserts the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="interceptorProviderResolver">The interceptor provider resolver.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">name</exception>
        public InterceptorProviderResolverCollection Insert(string name, IInterceptorProviderResolver interceptorProviderResolver, int index)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(interceptorProviderResolver, nameof(interceptorProviderResolver));
            if (_resolverDictionary.ContainsKey(name))
            {
                throw new ArgumentException("Duplicate InterceptorProviderResolver with the name {0} is registered.", nameof(name));
            }

            _resolverList.Insert(index,interceptorProviderResolver);
            _resolverDictionary.Add(name, interceptorProviderResolver);
            return this;
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">name</exception>
        public InterceptorProviderResolverCollection Remove(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            if (!_resolverDictionary.ContainsKey(name))
            {
                throw new ArgumentException("The InterceptorProviderResolver with name {0} is not registered.", nameof(name));
            }

            var provider = _resolverDictionary[name];
            _resolverDictionary.Remove(name);
            _resolverList.Remove(provider);
            return this;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns></returns>
        public InterceptorProviderResolverCollection Clear()
        {
            _resolverList.Clear();
            _resolverDictionary.Clear();
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IInterceptorProviderResolver> IEnumerable<IInterceptorProviderResolver>.GetEnumerator()
        {
            return _resolverList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resolverList.GetEnumerator();
        }
        #endregion
    }
}
