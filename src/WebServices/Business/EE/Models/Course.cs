using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.EE.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WhatYouWillLearn { get; set; }
        public bool Finished { get; set; } = false;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public double Price { get; set; }
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
