// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա��ڷ��ظ��ͻ��˵Ĳ�ѯ�����
/// </summary>
/// <typeparam name="T">��ѯ��������������ͣ��ο�<see cref="Items"/> </typeparam>
public interface IListResponse<T> : IResponseDto
{
    /// <summary>
    /// ��ѯ�����
    /// </summary>
    IReadOnlyList<T> Items { get; init; }
}
