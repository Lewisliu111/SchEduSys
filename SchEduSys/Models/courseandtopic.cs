//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchEduSys.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class courseandtopic
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public int topicId { get; set; }
    
        public virtual course course { get; set; }
        public virtual coursetopic coursetopic { get; set; }
    }
}
