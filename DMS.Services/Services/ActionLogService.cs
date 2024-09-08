using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Services.Services
{
    public class ActionLogService : IActionLogService
    {
        private readonly IUnitOfWork _uOW;

        public ActionLogService(IUnitOfWork UOW)
        {
            _uOW = UOW;
            
        }

        public async Task<IReadOnlyList<ActionLog>> GetAllActionLogs()
        {
            var allActionLogs = await _uOW.actionLogRepository.GetAllAsync();
            if(allActionLogs == null)
            {
                throw new Exception("No Action Logs Found");
            }
            return allActionLogs;
        }
    }
}
