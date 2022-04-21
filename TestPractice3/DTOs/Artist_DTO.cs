using System;
using System.Collections.Generic;

namespace TestPractice3.DTOs
{
    public class Artist_DTO
    {
        public int IdArtist { get; set; }
        public string Nickname { get; set; }
        public DateTime performanceDate { get; set; }
         public List<Organiser_DTO> Organisers { get; set; }


    }
}
