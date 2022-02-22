// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// ��׼����ѯ����ӿڣ��Ա������÷��ظ��ͻ��˵ķ�ҳ���������������
/// ����μ� <see cref="IListResponse{T}"/> �� <see cref="ITotalCountResponse"/>
/// </summary>
/// <typeparam name="T">��ѯ������������� <see cref="IListResponse{T}.Items"/></typeparam>
public interface IPagedResponse<T> : IListResponse<T>, ITotalCountResponse
{

}
