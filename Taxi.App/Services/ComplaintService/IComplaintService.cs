using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.ComplaintService
{
    public interface IComplaintService
    {
        Task<Guid> AddComplaint(Guid userId, ComplaintDTO complaintInfo);
        Task<List<ComplaintDTO>> GetUsersComplaints(Guid userId);

        //Role = admin
        Task<ComplaintDTO> ReviewComplaint(Guid userId, Guid complaintId);
        Task<List<ComplaintDTO>> GetAllComplaints(Guid userId);
        Task<List<ComplaintDTO>> GetComplaintsByTopic(Guid userId, ComplaintTopic? topic = null);
        Task<ComplaintDTO> ResolveComplaint(Guid userId, Guid complaintId, string adminComment); 
    }
}
