using Groups.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authorization.Consumer.Domain
{
    //RETURN GROUP with all information
    public class GroupV4 //: PaginatableObject
    {
        //private GroupTypeEnumV4 _type;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } //??????????????????????????

        /*[Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupTypeEnumV4 Type
        {
            get => _type;
            set
            {
                //_type = value;

                if (OrganizationID == 0)
                    _type = GroupTypeEnumV4.LEARNER;
                else
                    _type = GroupTypeEnumV4.ADMIN;
            }
        }*/

        [Required]
        public int OrganizationID { get; set; } = 0;

        /* [Required]
         public int[] OwnerID { get; set; } //????????????????*/

        public int PrimaryCategory { get; set; } = 0;
        public int SecondaryCategory { get; set; } = 0;

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupTypeEnumV4 Type { get; set; } //?????????????????????????????? --- Int

        [Required]
        [StringLength(100)] //NAO BATE COM A DOC, banco limitado a 100
        public string Title { get; set; }

        [StringLength(255)]
        public string Description { get; set; } = "";

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContextEnum Context { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupVisibilityEnum Visibility { get; set; } // ????????????? ALL OWNER MEMBERS NAO BATE COM A DOC

        // Note: API returns UNIX timestamps - use UnixTimeHelper to convert
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int UpdatedBy { get; set; }

        [Required]
        public List<GroupMembershipV4> Memberships { get; set; }

        //public override string GetUniqueID()
        //{
        //    return Id.ToString();

        //}
    }
}
