
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_app.Dtos
{
    public partial class TokenDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}
