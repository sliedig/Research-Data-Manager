using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Models
{
    public class ProvisioningViewModel
    {
        public ProvisioningViewModel()
        {
            ProvisioningInformation = new List<ProvisioningInformation>();
        }

        private IEnumerable<Project> _projects;

        [ReadOnly(true)]
        public IEnumerable<Project> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                foreach (var project in value)
                {
                    var provisioningInfo = ProvisioningInformation.SingleOrDefault(p => p.Id == project.Id);
                    if (provisioningInfo == null)
                    {
                        provisioningInfo = new ProvisioningInformation {Id = project.Id};
                        ProvisioningInformation.Add(provisioningInfo);
                    }
                    provisioningInfo.RequestDate = project.InitialProvisioningRequestDate;
                    provisioningInfo.ProvisioningStatus = project.ProvisioningStatus.ToDescription();
                    provisioningInfo.Title = project.Title;
                    provisioningInfo.RequestId = project.ProvisioningRequestId;
                    if (string.IsNullOrEmpty(provisioningInfo.SiteUrl))
                        provisioningInfo.SiteUrl = project.SiteUrl;
                }
            }
        }

        private List<ProvisioningInformation> _provisioningInformation;
        public List<ProvisioningInformation> ProvisioningInformation
        {
            get { _provisioningInformation.ForEach((i, p) => p.Item = i); return _provisioningInformation; }
            set { _provisioningInformation = value; }
        }
    }

    public class ProvisioningInformation
    {
        public int Item { get; set; }
        public int Id { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Title { get; set; }
        public string ProvisioningStatus { get; set; }
        public int RequestId { get; set; }
        public string SiteUrl { get; set; }
        public bool Select { get; set; }
    }
}
