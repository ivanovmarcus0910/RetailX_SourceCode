using BusinessObjectRetailX.Models;
using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public class StatisticRepository: IStatisticRepository
    {
        private readonly StatisticDAO statisticDAO;
        private readonly UserDAO userDAO;
        private readonly TenantDAO tenantDAO;
        public StatisticRepository(StatisticDAO statisticDAO, UserDAO userDAO, TenantDAO tenantDAO)
        {
            this.statisticDAO = statisticDAO;
            this.userDAO = userDAO;
            this.tenantDAO = tenantDAO;
        }

        public bool AddStatistic(Statistic statistic)
        {
            return statisticDAO.AddStatistic(statistic);
        }

        public bool CheckStatisticExists(string day)
        {
            return statisticDAO.GetStatisticByDay(day) != null;
        }

        public bool DeleteStatistic(int id)
        {
            return statisticDAO.DeleteStatistic(id);
        }

        public void EnsureStatisticForToday()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (!CheckStatisticExists(today))
            {
                Statistic temp = new Statistic
                {
                    Day = today,
                    TotalTenantActive = tenantDAO.QuantityTenantActive(),
                    TotalTenant = tenantDAO.QuantityTenant(),
                    TotalUserActive = userDAO.QuantityUserActive(),
                    TotalUser = userDAO.QuantityUser(),
                    AccessCount = 0,
                };
                AddStatistic(temp);
            }
        }

        public Statistic GetStatistic(string day)
        {
            return statisticDAO.GetStatisticByDay(day);
        }

        public Statistic GetStatisticById(int id)
        {
            return statisticDAO.GetStatisticById(id);
        }

        public List<Statistic> GetStatisticList()
        {
            return statisticDAO.GetStatisticList();
        }

        public void IncreaseCount()
        {
            Console.WriteLine("Tăng Access Count");
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (!CheckStatisticExists(today))
            {
                EnsureStatisticForToday();
                IncreaseCount();
            }
            Statistic temp = GetStatistic(today);
            temp.AccessCount += 1;
            UpdateStatistic(temp);
        }

        public bool UpdateStatistic(Statistic statistic)
        {
            return statisticDAO.UpdateStatistic(statistic);
        }
    }
}
