using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Repository
{
    public class CaloriesPredictionRepository :BaseRepository<CaloriesPrediction> , ICaloriesPredictionRepository
    {
        private readonly HealthMonitoringContext _dbcontext;

        public CaloriesPredictionRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));

        }

        public async Task<CaloriesPrediction> GetUserDayPredictionAsync(string userId, int day)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
            return await _dbcontext.CaloriesPredictions
                .Include(c => c.ActivityData)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ActivityData.Day == day);
        }

        public async Task<IEnumerable<CaloriesPrediction>> GetUserPredictionsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
            return await _dbcontext.CaloriesPredictions
                .Where(c => c.UserId == userId)
                .Select(c => new CaloriesPrediction
                {
                   // UserId = c.UserId,
                    PredictedCalories = c.PredictedCalories,
                    PredictionDate = c.PredictionDate
                })
                .ToListAsync();
        }
        public async Task<CaloriesPrediction> GetLatestCaloriesPredictionByUserIdAsync(string userId)
        {
            return await _dbcontext.CaloriesPredictions
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.PredictionDate)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<CaloriesPrediction>> GetCaloriesPredictionByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            IQueryable<CaloriesPrediction> query = _dbset;
            return await query.Where(h => h.UserId == userId &&
                           h.CreatedAt >= startDate &&
                           h.CreatedAt <= endDate)
                            .OrderBy(h => h.CreatedAt)
                            .ToListAsync();
        }
    }
}
