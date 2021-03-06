﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeavyBoot.Api.Model
{
    public class HBDataTable
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        [Key]
        public string Pcname { get; set; }

        public DateTime DateServer { get; set; }

        public DateTime ClientTime { get; set; }

        public DateTime ImportTime { get; set; }

        public DateTime ExportTime { get; set; }

        public byte TimeOut { get; set; }

        public bool IsChecked { get; set; }
    }
}
