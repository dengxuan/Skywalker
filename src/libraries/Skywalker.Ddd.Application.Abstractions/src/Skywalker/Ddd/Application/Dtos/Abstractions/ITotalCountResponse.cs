// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա������÷��ظ��ͻ��˵���������
/// </summary>
public interface ITotalCountResponse
{
    /// <summary>
    /// ���ϲ�ѯ��������������
    /// </summary>
    int TotalCount { get; init; }
}
