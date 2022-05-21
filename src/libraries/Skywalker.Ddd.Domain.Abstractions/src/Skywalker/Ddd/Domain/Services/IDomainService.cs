using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Extensions.Collections.Generic;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �����һ������һ���������
/// </summary>
public interface IDomainService : ITransientDependency { }

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �ṩ���������ݲ����ӿ�
/// </summary>
public interface IDomainService<TEntity> : IDomainService where TEntity : class, IEntity
{
    /// <summary>
    /// ���һ��ʵ��
    /// </summary>
    /// <param name="entity">����ӵ�ʵ��</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>��Ӻ��ʵ��</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����һ��ʵ��
    /// </summary>
    /// <param name="entity">Ҫ���µ�ʵ��</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>���º��ʵ��</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ɾ��һ��ʵ��
    /// </summary>
    /// <param name="entity">Ҫɾ����ʵ��</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡʵ������
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡʵ������
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������<paramref name="filter"/>��ʵ������
    /// </summary>
    /// <param name="filter">����ɸѡ����</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������<paramref name="filter"/>��ʵ���Ƿ����
    /// </summary>
    /// <param name="filter">ɸѡ����</param>
    /// <param name="cancellationToken"></param>
    /// <returns>���ڷ���true�����򷵻�false</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������<paramref name="expression"/>��ʵ������
    /// </summary>
    /// <param name="expression">����ɸѡ����</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ѯ����ʵ�壬����ʹ�á�
    /// ����Ͻ�ʹ�ô˷������Ƽ�ʹ��<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>���з�ҳ��ȡ
    /// </summary>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>��ǰʵ��<see cref="TEntity"/>���������ݼ������߿ռ���</returns>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ѯ��������������ʵ�弯������ʹ�á�
    /// ɸѡ����϶�ʱ���Ƽ�ʹ��<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>���з�ҳ��ȡ
    /// </summary>
    /// <param name="expression">��ѯ����</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>��ǰʵ��<see cref="TEntity"/>���������ݼ������߿ռ���</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ҳ��ѯ��ǰʵ��<see cref="TEntity"/>�����ݼ�.
    /// Ĭ�ϰ�����ʱ������,���ʵ��δʵ��<see cref="IHasCreationTime"/>�ӿ�,����Id����
    /// </summary>
    /// <param name="skip">�������������������</param>
    /// <param name="limit">����ȡ��������</param>
    /// <param name="sorting">��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ ���磺 "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>�������������ݼ�</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ҳ��ѯ��ǰʵ��<see cref="TEntity"/>�����ݼ�
    /// </summary>
    /// <param name="skip">�������������������</param>
    /// <param name="limit">����ȡ��������</param>
    /// <param name="sorting">��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ ���磺 "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>�������������ݼ�</returns>
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������ҳ��ѯʵ��<see cref="TEntity"/>���ݼ�
    /// Ĭ�ϰ�����ʱ������,���ʵ��δʵ��<see cref="IHasCreationTime"/>�ӿ�,����Id����
    /// </summary>
    /// <param name="expression">ɸѡ����</param>
    /// <param name="skip">�������������������</param>
    /// <param name="limit">����ȡ��������</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns></returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������ҳ��ѯʵ��<see cref="TEntity"/>���ݼ�
    /// </summary>
    /// <param name="expression">ɸѡ����</param>
    /// <param name="skip">�������������������</param>
    /// <param name="limit">����ȡ��������</param>
    /// <param name="sorting">��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ ���磺 "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns></returns>
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> filter, int skip, int limit, string sorting, CancellationToken cancellationToken = default);
}

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �ṩ���������ݲ����ӿ�
/// </summary>
public interface IDomainService<TEntity, TKey> : IDomainService<TEntity> where TEntity : class, IEntity<TKey> where TKey : notnull
{

    /// <summary>
    /// ��ȡ��������<paramref name="id"/>��ʵ���Ƿ����
    /// </summary>
    /// <param name="id">ʵ����</param>
    /// <param name="cancellationToken"></param>
    /// <returns>���ڷ���true�����򷵻�false</returns>
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����������ʵ�塣
    /// �������������ʵ�岻���ڣ����׳� <see cref="EntityNotFoundException"/> �쳣
    /// </summary>
    /// <param name="id">Ҫ��ȡʵ�������</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <exception cref="EntityNotFoundException">����������ʵ�岻����ʱ�׳�</exception>
    /// <returns>����������ʵ��<see cref="TEntity"/></returns>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����������ʵ�壬���û���ҵ�����null
    /// </summary>
    /// <param name="id">Ҫ��ȡʵ�������</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>����������ʵ��<see cref="TEntity"/>����null</returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

}
