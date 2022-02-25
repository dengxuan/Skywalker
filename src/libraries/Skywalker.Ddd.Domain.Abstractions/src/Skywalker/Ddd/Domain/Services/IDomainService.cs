using System.Linq.Expressions;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Services;

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �����һ������һ���������
/// </summary>
public interface IDomainService
{
}

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �ṩ���������ݲ����ӿ�
/// </summary>
public interface IDomainService<TEntity> : IDomainService<TEntity, Guid> where TEntity : class, IEntity<Guid>
{
    /// <summary>
    /// ����һ��ʵ��
    /// </summary>
    /// <param name="entity">Ҫ���µ�ʵ��</param>
    /// <returns>���º��ʵ��</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// ɾ��һ��ʵ��
    /// </summary>
    /// <param name="entity">Ҫɾ����ʵ��</param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// ��ѯ����ʵ�壬����ʹ�á�
    /// ����Ͻ�ʹ�ô˷������Ƽ�ʹ��<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>���з�ҳ��ȡ
    /// </summary>
    /// <returns>��ǰʵ��<see cref="TEntity"/>���������ݼ������߿ռ���</returns>
    Task<List<TEntity>> GetListAsync();

    /// <summary>
    /// ��ѯ��������������ʵ�弯������ʹ�á�
    /// ɸѡ����϶�ʱ���Ƽ�ʹ��<see cref="IDomainService{TEntity, TKey}.GetPagedListAsync(int, int, string, CancellationToken)"/>���з�ҳ��ȡ
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>��ǰʵ��<see cref="TEntity"/>���������ݼ������߿ռ���</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// ��ҳ��ѯ��ǰʵ��<see cref="TEntity"/>�����ݼ�
    /// </summary>
    /// <param name="skipCount">�������������������</param>
    /// <param name="maxResultCount">����ȡ��������</param>
    /// <param name="sorting">��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ ���磺 "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns>�������������ݼ�</returns>
    Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������ҳ��ѯʵ��<see cref="TEntity"/>���ݼ�
    /// </summary>
    /// <param name="expression">ɸѡ����</param>
    /// <param name="skipCount">�������������������</param>
    /// <param name="maxResultCount">����ȡ��������</param>
    /// <param name="sorting">��Ҫ���������ֶκͿ�ѡ������ָ��(ASC �� DESC)���ɰ�������ֶΣ�ʹ�ö���(,)�ָ ���磺 "Name ASC, Age DESC"</param>
    /// <param name="cancellationToken">��ȡ������������</param>
    /// <returns></returns>
    Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);
}

/// <summary>
/// ��������񶼿���ʵ�ִ˽ӿڣ��԰�Լ��ʶ������.
/// �ṩ���������ݲ����ӿ�
/// </summary>
public interface IDomainService<TEntity, TKey> : IDomainService where TEntity : class, IEntity<TKey> where TKey : notnull
{

    /// <summary>
    /// ��ȡ����������ʵ�塣
    /// �������������ʵ�岻���ڣ����׳� <see cref="EntityNotFoundException"/> �쳣
    /// </summary>
    /// <param name="id">Ҫ��ȡʵ�������</param>
    /// <exception cref="EntityNotFoundException">����������ʵ�岻����ʱ�׳�</exception>
    /// <returns>����������ʵ��<see cref="TEntity"/></returns>
    Task<TEntity> GetAsync(TKey id);

    /// <summary>
    /// ��ȡ����������ʵ�壬���û���ҵ�����null
    /// </summary>
    /// <param name="id">Ҫ��ȡʵ�������</param>
    /// <returns>����������ʵ��<see cref="TEntity"/>����null</returns>
    Task<TEntity?> FindAsync(TKey id);

}
