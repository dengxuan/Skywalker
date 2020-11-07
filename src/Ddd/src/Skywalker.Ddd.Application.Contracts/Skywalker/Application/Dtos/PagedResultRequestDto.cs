﻿using Skywalker.Application.Dtos.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace Skywalker.Application.Dtos
{
    /// <summary>
    /// Simply implements <see cref="IPagedResultRequest"/>.
    /// </summary>
    [Serializable]
    public class PagedResultRequestDto : LimitedResultRequestDto, IPagedResultRequest
    {
        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; }
    }
}