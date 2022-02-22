// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա������öԲ�ѯ���������
/// </summary>
public interface ISortedRequest
{
    /// <summary>
    /// ������Ϣ��
    /// ��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ�.
    /// </summary>
    /// <example>
    /// ����:
    /// "Name"
    /// "Name DESC"
    /// "Name ASC, Age DESC"
    /// </example>
    string Sorting { get; init; }
}
