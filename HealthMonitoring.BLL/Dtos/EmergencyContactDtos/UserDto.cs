﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.EmergencyContactDtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<EmergencyContactDto> EmergencyContacts { get; set; } = new List<EmergencyContactDto>();
    }
}
