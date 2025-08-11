using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramManager.Interfaces
{
    interface IDatabase<T> {
        // 테이블에 대한 모든 DATA 조회
        List<T>? Get();

        // 테이블에 대해 특정 ID에 해당하는 DATA 조회
        T? GetDetail(int? id);

        // 테이블에 특정 DATA DELETE
        void Delete(int? id);
    }
}
