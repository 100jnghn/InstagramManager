using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramManager.Interfaces
{
    public interface IDatabase<T>
    {
        // 테이블의 모든 데이터 조회
        List<T> GetAllData();

        // 테이블에 값 Insert
        void InsertData(T entity);

        // 테이블의 특정 id에 해당하는 데이터 조회
        T? GetData(string id);

        // 테이블의 특정 id에 해당하는 데이터의 information Update
        void UpdateData(T entity);

        // 테이블의 데이터 삭제
        void DeleteData(string id);
    }
}
