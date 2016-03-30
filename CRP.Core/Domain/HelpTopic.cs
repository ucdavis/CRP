using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class HelpTopic : DomainObject
    {
        public HelpTopic()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            AvailableToPublic = false;
            IsActive = true;
            NumberOfReads = 0;
        }
        [Required]
        public virtual string Question { get; set; }
        public virtual string Answer { get; set; }
        public virtual bool AvailableToPublic { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int NumberOfReads { get; set; }
        public virtual bool IsVideo { get; set; }
        [StringLength(50)]
        public virtual string VideoName { get; set; }

        #region Complex Validation. Fields not in database

        [AssertTrue(Message = "VideoName required when IsVideo selected")]
        public virtual bool IsVideoNeedsVideoName
        {
            get
            {
                if (IsVideo)
                {
                    if(string.IsNullOrEmpty(VideoName) || VideoName.Trim() == string.Empty)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion Complex Validation. Fields not in database
    }
}
