// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա�����������ѯ�������
/// </summary>
public interface ILimitedRequest
{
    /// <summary>
    /// Ӧ��������ѯ���������ͨ���������Ʒ�ҳ�Ĳ�ѯ�������
    /// </summary>
    int Limit { get; init; }
}
