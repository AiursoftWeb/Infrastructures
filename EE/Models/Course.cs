using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseImage { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WhatYouWillLearn { get; set; } = string.Empty;
        public bool Finished { get; set; } = false;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public double Price { get; set; } = 0;
        public bool DisplayOwnerInfo { get; set; }


        [InverseProperty(nameof(Section.Context))]
        public IEnumerable<Section> Sections { get; set; }

        [InverseProperty(nameof(Subscription.Course))]
        public IEnumerable<Subscription> Subscribers { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public EEUser Owner { get; set; }
    }
}
