﻿using System;
using System.ComponentModel.DataAnnotations;

namespace HeavyBoot.DAL.Entitys
{
    public class HBDataTable
    {
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
