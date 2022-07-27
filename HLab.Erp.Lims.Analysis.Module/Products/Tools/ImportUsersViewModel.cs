using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Windows.Controls;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Users
{
    public class AdUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Function { get; set; }
        public string Email { get; set; }

        public override string ToString() => Login + " : " + FirstName + " " + LastName;
    }
}
