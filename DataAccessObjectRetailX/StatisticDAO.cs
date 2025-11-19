using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjectRetailX
{
    public class StatisticDAO
    {
        private readonly RetailXContext _context;
        public StatisticDAO(RetailXContext context)
        {
            _context = context;
        }
        public Statistic GetStatisticById(int id)
        {
            return _context.Statistics.Find(id);
        }
        public Statistic GetStatisticByDay(string day)
        {
            Statistic? statistic = null;
            try
            {
                  statistic = _context.Statistics.FirstOrDefault(s => s.Day == day);
                
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the statistic.", ex);
            }
            return statistic;
        }
        public List<Statistic> GetStatisticList()
        {
            return _context.Statistics.ToList();
        }
        public bool AddStatistic(Statistic statistic)
        {
            try
            {
                _context.Statistics.Add(statistic);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateStatistic(Statistic statistic)
        {
            try
            {
                _context.Statistics.Update(statistic);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteStatistic(int id)
        {
            var statistic = _context.Statistics.Find(id);
            try
            {
                _context.Statistics.Remove(statistic);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
