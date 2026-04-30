<<<<<<< HEAD
﻿using Domain.Entities;
using System;
=======
﻿using System;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using Domain.Entities;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97

namespace Application.Interfaces
{
    public interface ISeatRepository
    {
<<<<<<< HEAD
            Task<List<SEAT>> GetBySectorIdAsync(int sectorId);
=======
        Task<List<SEAT>> GetBySectorIdAsync(int sectorId);
        Task<SEAT?> GetByIdAsync(Guid id);
         Task UpdateAsync(SEAT seat);
        Task SaveChangesAsync();

>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
    }
}
