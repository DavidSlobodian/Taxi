using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.ComplaintService
{
    public class ComplaintsService : IComplaintService
    {
        public Task<Guid> AddComplaint(Guid userId, ComplaintDTO complaintInfo)
        {
            throw new NotImplementedException();
        }

        public Task<List<ComplaintDTO>> GetAllComplaints(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ComplaintDTO>> GetComplaintsByTopic(Guid userId, ComplaintTopic? topic = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<ComplaintDTO>> GetUsersComplaints(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ComplaintDTO> ResolveComplaint(Guid userId, Guid complaintId, string adminComment)
        {
            throw new NotImplementedException();
        }

        public Task<ComplaintDTO> ReviewComplaint(Guid userId, Guid complaintId)
        {
            throw new NotImplementedException();
        }
    }
}
