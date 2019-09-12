﻿using Aiursoft.Pylon.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class CreateSiteViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public CreateSiteViewModel()
        {
        }

        public CreateSiteViewModel(ColossusUser user) : base(user, 0, "Quick upload")
        {
        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, 0, "Quick upload");
        }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidFolderName]
        public string SiteName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidFolderName]
        public string AdminUserName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        [ValidFolderName]
        public string AdminPassword { get; set; }

        public string ChoseID { get; set; }
    }
}