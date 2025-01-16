using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DischargerV2.Database
{
    [Table("TblUserInfo")]
    public class TblUserInfo
    {
        [Key]
        [Column("userid")]
        public string UserId { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("username")]
        public string UserName { get; set; } 
        [Column("create_dt")]
        public DateTime CreateDt { get; set; }
        [Column("isAdmin")]
        public bool IsAdmin { get; set; }

        public TblUserInfo() { }

        public TblUserInfo(string UserId, string Password, string UserName, bool IsAdmin)
        {
            this.UserId = UserId;
            this.Password = Password;
            this.UserName = UserName;
            this.IsAdmin = IsAdmin;
        }
    }
}
