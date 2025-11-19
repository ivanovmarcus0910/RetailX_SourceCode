using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public interface IStatisticRepository
    {
        Statistic GetStatistic(string day);
        Statistic GetStatisticById(int id);
        List<Statistic> GetStatisticList();
        bool AddStatistic(Statistic statistic);
        bool UpdateStatistic(Statistic statistic);
        bool DeleteStatistic(int id);
        bool CheckStatisticExists(string day);

        void EnsureStatisticForToday();
        void IncreaseCount();

    }
}
