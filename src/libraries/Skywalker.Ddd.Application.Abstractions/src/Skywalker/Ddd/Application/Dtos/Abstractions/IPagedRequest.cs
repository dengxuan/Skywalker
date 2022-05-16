// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա�ʵ�ֲ�ѯ��ҳ
/// </summary>
public interface IPagedRequest : ILimitedRequest
{
    /// <summary>
    /// ������������(��ҳ�Ŀ�ʼ����)
    /// </summary>
    int Skip { get; init; }
}
