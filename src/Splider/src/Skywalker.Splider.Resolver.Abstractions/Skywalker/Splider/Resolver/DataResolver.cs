﻿using Microsoft.Extensions.Logging;
using Skywalker.Splider.Http;
using Skywalker.Splider.Pipelines;
using Skywalker.Splider.Resolver.Selector;
using Skywalker.Splider.Resolver.Selector.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skywalker.Splider.Resolver
{
    public abstract class DataResolver : Pipeline
    {
        private readonly List<Func<PipelineContext, ISelectable, IEnumerable<Request>>> _followRequestQueriers;
        private readonly List<Func<Request, bool>> _requiredValidator;

        /// <summary>
        /// 选择器的生成方法
        /// </summary>
        public Func<PipelineContext, ISelectable> SelectableBuilder { get; protected set; }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns></returns>
        protected abstract Task ParseAsync(PipelineContext context, ISelectable selectable);

        protected DataResolver(ILogger<DataResolver> logger) : base(logger)
        {
            _followRequestQueriers = new List<Func<PipelineContext, ISelectable, IEnumerable<Request>>>();
            _requiredValidator = new List<Func<Request, bool>>();
        }

        public virtual void AddFollowRequestQuerier(PipelineContext context, ISelector selector)
        {
            _followRequestQueriers.Add((context, selectable) =>
            {
                var requests = selectable.SelectList(selector)
                    .Where(x => x != null)
                    .SelectMany(x => x.Links())
                    .Select(x =>
                    {
                        var request = context.CreateNewRequest(new Uri(x));
                        request.RequestedTimes = 0;
                        return request;
                    });
                return requests;
            });
        }

        public virtual void AddRequiredValidator(Func<Request, bool> requiredValidator)
        {
            _requiredValidator.Add(requiredValidator);
        }

        public virtual void AddRequiredValidator(string pattern)
        {
            _requiredValidator.Add(request => Regex.IsMatch(request.RequestUri.ToString(), pattern));
        }

        //protected virtual void AddParsedResult<T>(PipelineContext context, IEnumerable<T> results)
        //    where T : EntityBase<T>, new()
        //{
        //    if (results == null)
        //    {
        //        return;
        //    }

        //    var type = typeof(T);
        //    var items = context.GetData(type);
        //    if (items == null)
        //    {
        //        var list = new List<T>();
        //        list.AddRange(results);
        //        context.AddData(type, list);
        //    }
        //    else
        //    {
        //        items.AddRange(results);
        //    }
        //}

        internal void UseHtmlSelectableBuilder()
        {
            SelectableBuilder = context =>
            {
                var text = context.Response.ReadAsString().TrimStart();
                return CreateHtmlSelectable(context, text);
            };
        }

        private ISelectable CreateHtmlSelectable(PipelineContext context, string text)
        {
            var uri = context.Request.RequestUri;
            var domain = uri.Port == 80 || uri.Port == 443
                ? $"{uri.Scheme}://{uri.Host}"
                : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
            return new HtmlSelectable(text, domain, context.Options.RemoveOutboundLinks);
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns></returns>
        public override async Task HandleAsync(PipelineContext context)
        {
            context.NotNull(nameof(context));
            context.Response.NotNull(nameof(context.Response));

            if (!IsValidRequest(context.Request))
            {
                Logger.LogInformation(
                    $"{GetType().Name} ignore parse request {context.Request.RequestUri}, {context.Request.Hash}");
                return;
            }
            ISelectable selectable = null;
            //if (context.Selectable == null)
            //{
            //    if (SelectableBuilder != null)
            //    {
            //        context.Selectable = SelectableBuilder(context);
            //    }
            //    else
            //    {
            //        var text = context.Response.ReadAsString().TrimStart();
            //        if (text.StartsWith("<!DOCTYPE html>") || text.StartsWith("<html>"))
            //        {
            //            context.Selectable = CreateHtmlSelectable(context, text);
            //        }
            //        else
            //        {
            //            try
            //            {
            //                var token = (JObject)JsonConvert.DeserializeObject(text);
            //                context.Selectable = new JsonSelectable(token);
            //            }
            //            catch
            //            {
            //                context.Selectable = new TextSelectable(text);
            //            }
            //        }
            //    }
            //}

            await ParseAsync(context, selectable);

            var requests = new List<Request>();

            if (_followRequestQueriers != null)
            {
                foreach (var followRequestQuerier in _followRequestQueriers)
                {
                    var followRequests = followRequestQuerier(context, selectable);
                    if (followRequests != null)
                    {
                        requests.AddRange(followRequests);
                    }
                }
            }

            foreach (var request in requests)
            {
                if (IsValidRequest(request))
                {
                    // 在此强制设制 Owner, 防止用户忘记导致出错
                    request.Owner = context.Request.Owner;
                    request.Agent = context.Response.Agent;
                    context.AddFollowRequests(request);
                }
            }
        }

        public bool IsValidRequest(Request request)
        {
            return _requiredValidator.Count <= 0 ||
                   _requiredValidator.Any(validator => validator(request));
        }
    }
}